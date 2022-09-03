using UnityEngine;

/// Based on tutorial by Code Monkey
/// https://www.youtube.com/watch?v=xuCtxIcfboM
/// ---------------------------------------
/// I found a better tutorial anyway,
/// check BetterCarAIController class
///                               - K
/// ---------------------------------------
///      THIS CLASS IS DEPRECATED


public class CarAIController : MonoBehaviour
{
    float turnThreshold;
    float distanceThreshold;

    // Used by AIFrontTrigger to check if it's colliding with anything but cars,
    // in which case it sets it to true. Otherwise it's kept as "false".
    [HideInInspector] public bool needsToReverse = false;

    PlayerMovement pm;
    Vector3 targetPosition;
    AICheckpoint[] checkpoints;
    int nextCheckpoint = 0;
    Rigidbody rb;

    // Obstacle detection
    private Ray[] rays =
    {
        new Ray(), // front left
        new Ray(), // front center
        new Ray(), // front right
        new Ray()  // back
    };
    Vector3 origin;
    Vector3[] directions;

    float detectionDistance;
    float carDetectionDistance;
    float[] distances;
    readonly int layerMask = 1 << 7;
    readonly int carLayerMask = 1 << 6;
    readonly float root2 = Mathf.Sqrt(2);
    bool[] hits, carHits;
    bool ignoreDistanceThreshold;

    // Debug
    readonly bool debug = true;
    Color debugColor;

    private void Awake()
    {
        AICheckpoints _;
        try
        {
            _ = GameObject.Find("Bot checkpoints").GetComponent<AICheckpoints>();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogWarning("No bot checkpoints in this scene");
            _ = null;
        }
        if (_ != null)
        {
            checkpoints = _.checkpoints;
            targetPosition = checkpoints[0].checkpoint.position;
            ignoreDistanceThreshold = checkpoints[0].ignoreDistanceThreshold;
            turnThreshold = _.turnThreshold;
            distanceThreshold = _.distanceThreshold;
            detectionDistance = _.detectionDistance;
        }
        rb = GetComponent<Rigidbody>();
        directions = new Vector3[4];
        hits = new bool[4];
        carHits = new bool[4];
        distances = new float[4];
        pm = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (debug) debugColor = new Color(Random.value, Random.value, Random.value);
    }

    private void Update()
    {
        if (checkpoints != null)
        {
            float forwardAmount;
            float turnAmount;
            float velocity = rb.velocity.magnitude;

            // Try to avoid obstacles whenever needed
            // - Update rays
            origin = transform.position + transform.up;

            directions = new Vector3[] {
            (transform.forward * (1 + velocity / 20) - transform.right).normalized,
            transform.forward,
            (transform.forward * (1 + velocity / 20) + transform.right).normalized,
            -transform.forward
        };

            distances = new float[]
            {
            detectionDistance * root2,
            detectionDistance,
            detectionDistance * root2,
            detectionDistance
            };

            for (int i = 0; i < 4; i++)
            {
                rays[i].origin = origin;
                rays[i].direction = directions[i];
            }

            carDetectionDistance = 4 * (velocity / 40) + 4;

            // - Perform raycasts

            int x = 0;
            foreach (Ray ray in rays)
            {
                hits[x] = Physics.Raycast(ray, distances[x], layerMask);
                carHits[x] = Physics.Raycast(ray, carDetectionDistance, carLayerMask);
                x++;
            }
            // Debug
            if (debug)
            {
                foreach (Ray ray in rays)
                {
                    Debug.DrawRay(ray.origin, ray.direction * detectionDistance, debugColor);
                }
            }


            // Accelerate/Brake
            // Get the dot product of forward vector and direction to next checkpoint to see
            // whether it's better to drive forwards or in reverse.

            Vector3 dirToMovePosition = (targetPosition - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToMovePosition);
            float dotThreshold = pm.isOnAntigrav ? -0.89f : -0.45f;

            if (dot > dotThreshold)
            {
                if (checkpoints[nextCheckpoint].needsDesiredSpeed && transform.InverseTransformDirection(pm.rb.velocity).z > checkpoints[nextCheckpoint].desiredSpeed)
                {
                    forwardAmount = -1;
                }
                else forwardAmount = 1;
            }
            else forwardAmount = -1;


            // Turn
            // Check for angle to see which way the car must turn to get to next checkpoint.
            // If it's below turnThreshold, it'll drive straight.
            Vector3 axis = pm.isOnAntigrav ? transform.up : Vector3.up;
            float angle = Vector3.SignedAngle(transform.forward, dirToMovePosition, axis);
            float distance = Vector3.Distance(targetPosition, transform.position);

            if (Mathf.Abs(angle) >= turnThreshold)
                turnAmount = angle > 0 ? 1 : -1;
            else turnAmount = 0;

            // Avoid obstacles and other cars

            if (hits[0] || carHits[0]) turnAmount = .3f;
            else if (hits[2] || carHits[2]) turnAmount = -.3f;
            if (hits[1] || carHits[1])
            {
                forwardAmount = -1;
                turnAmount = -Mathf.Sign(angle);
            }
            else if (hits[3] || carHits[3])
            {
                forwardAmount = 1;
                turnAmount = Mathf.Sign(angle);
            }

            // If distance is below distanceThreshold, pathfind to next checkpoint.

            if (distance < distanceThreshold && !ignoreDistanceThreshold)
            {
                SetTargetPosition(checkpoints[(nextCheckpoint + 1) % checkpoints.Length].checkpoint.position);
            }
            pm.SetAxisAI(forwardAmount, turnAmount);
        }
    }

    public void ResetCar()
    {
        targetPosition = checkpoints[0].checkpoint.position;
        nextCheckpoint = 0;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
    }
    // This is for checking if the AI car drove into a bot checkpoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bot Checkpoint") && other.transform.position == checkpoints[nextCheckpoint].checkpoint.position)
        {
            nextCheckpoint = (nextCheckpoint + 1) % checkpoints.Length;
            ignoreDistanceThreshold = checkpoints[nextCheckpoint].ignoreDistanceThreshold;
            SetTargetPosition(checkpoints[nextCheckpoint].checkpoint.position);
        }
    }
}
