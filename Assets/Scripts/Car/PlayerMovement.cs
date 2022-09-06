using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CarBrain car;

    public bool controlable;
    public string carName, carDescription;

    // Input system stuff
    GlobalInputs input;
    float a_horizontal, a_vertical, a_drift;
    public enum Axis { Horizontal, Vertical, Drift };

    public Renderer carBody;
    public WheelCollider rr, rl, fr, fl;

    public Transform body, allWheels;
    public float driftAngle;
    const float turnSmoothTime = .2f;
    float h; // Horizontal axis after smoothing, to mimic the Input.GetAxis("Horizontal") behaviour
    float driftDirection;
    bool isDrifting;

    public CarStats stats;

    [HideInInspector] float maxSpeed, acceleration, minSteerAngle, maxSteerAngle, brakeTorque, idleDecceleration, downforce;
    [HideInInspector] public float accelRate = 1;
    public Transform trr, trl, tfr, tfl;
    public bool reversing = false;
    public bool isOnAntigrav = false;
    Vector3 respawnPosition, startPosition, localVelocity;
    Quaternion respawnRotation, startRotation;
    float temp_vel, wheelRotationOffset;
    bool isResetting = false;

    //Practice mode
    public bool isInPractice = false;
    public int currentCheckpoint = 0;

    private bool hasCustomCheckpoint = false;
    private Vector3 customCheckpointPosition, levelCheckpointPosition;
    private Quaternion customCheckpointRotation, levelCheckpointRotation;

    private Vector3 savedVelocityCustom, savedAngularVelocityCustom, savedVelocityLevel, savedAngularVelocityLevel;
    private float[] motorTorques = new float[4];
    private float savedTime;

    [SerializeField] AudioSource engineSound;
    Pause pause;

    SceneLoaded carLoader;
    float resetTimer;

    private void Awake()
    {
        car = GetComponent<CarBrain>();

        try
        {
            pause = FindObjectOfType<Pause>();
        }
        catch(NullReferenceException)
        {
            pause = null;
        }

        int c;
        if (car.IsAI) c = UnityEngine.Random.Range(0, 14);
        else if (PlayerPrefs.HasKey("Car Colour")) c = PlayerPrefs.GetInt("Car Colour");
        else c = 0;
        carBody.material.color = CustomizationEncoder.GetColour(c);
        try
        {
            input = FindObjectOfType<GlobalInputs>();
        }
        catch (NullReferenceException)
        {
            input = null;
        }
    }
    // Input system stuff

    public void UpdateAxis(float input, Axis axis)
    {
        switch (axis)
        {
            case Axis.Horizontal:
                a_horizontal = input;
                break;
            case Axis.Vertical:
                a_vertical = input;
                break;
            case Axis.Drift:
                a_drift = input;
                break;
        }
    }
    private void Start()
    {
        // Respawn
        respawnPosition = transform.position;
        startPosition = transform.position;
        respawnRotation = transform.rotation;
        startRotation = transform.rotation;

        // Get stats
        maxSpeed = stats.maxSpeed;
        acceleration = stats.acceleration;
        minSteerAngle = stats.minSteerAngle;
        maxSteerAngle = stats.maxSteerAngle;
        brakeTorque = stats.brakeTorque;
        idleDecceleration = stats.idleDecceleration;
        downforce = stats.downforce;

        // Rigidbody stuff
        car.CRigidbody.centerOfMass = new Vector3(0f, -0.2f, 0f);

        // Other stuff
        try
        {
            carLoader = GameObject.Find("CarLoader").GetComponent<SceneLoaded>();
        }
        catch (NullReferenceException) { }


        // Input system stuff
        if (!car.IsAI && input != null)
        {
            input.input.Player.Reset.performed += ctx => ResetCar();
            input.input.Player.Drift.performed += ctx => UpdateAxis(ctx.ReadValue<float>(), Axis.Drift);
            input.input.Player.MoveForwBack.performed += ctx => UpdateAxis(ctx.ReadValue<float>(), Axis.Vertical);
            input.input.Player.MoveLeftRight.performed += ctx => UpdateAxis(ctx.ReadValue<float>(), Axis.Horizontal);
            input.input.Player.CheckpointAdd.performed += ctx => createCheckpoint(true);
            input.input.Player.CheckpointRemove.performed += ctx => removeLastCustomCheckpoint();
        }
    }
    private void Update()
    {
        UpdateWheel(rr, trr);
        UpdateWheel(rl, trl);
        UpdateWheel(fr, tfr);
        UpdateWheel(fl, tfl);
        UpdateDrift();
        engineSound.pitch = Mathf.LerpUnclamped(1, 3, temp_vel / 50);
        engineSound.volume = Mathf.Lerp(.3f, 1, Mathf.Abs(temp_vel) / 50);
        if (pause != null)
        {
            if (pause.paused)
            {
                if (engineSound.isPlaying) engineSound.Pause();
            }
            else
            {
                if (!engineSound.isPlaying) engineSound.Play();
            }
        }

        // Set the respawn point to the previous point on path
        var respawnIdx = Math.Clamp(car.CPosition.pointOnPath - 1, 0, car.Path.NumPoints - 1);
        respawnPosition = car.Path.GetPoint(respawnIdx);
        var t = car.Path.GetClosestTimeOnPath(respawnPosition);
        respawnRotation = car.Path.GetRotation(t) * Quaternion.Euler(0, 0, 90);

        // If bot is barely moving for 3 seconds, reset its position.
        if (car.IsAI)
        {
            if (car.CRigidbody.velocity.magnitude >= 0.1f)
            {
                resetTimer = Time.time + 3;
            }
            if (Time.time > resetTimer)
            {
                ResetCar();
                resetTimer = Time.time + 3;
            }
        }
    }

    // Used with BetterCarAIController
    public void SetAxisAI(float forward, float turn)
    {
        a_horizontal = turn;
        a_vertical = forward;
    }

    private void UpdateDrift()
    {
        Vector3 bodyTarget, wheelsTarget;
        if (isDrifting)
        {
            wheelsTarget = new Vector3(0, driftAngle * driftDirection, 0);
            bodyTarget = new Vector3(0, 90 + (driftAngle * driftDirection), 0);
        }
        else
        {
            wheelsTarget = Vector3.zero;
            bodyTarget = new Vector3(0, 90, 0);
        }
        allWheels.localRotation = Quaternion.Lerp(allWheels.localRotation, Quaternion.Euler(wheelsTarget), .1f);
        body.localRotation = Quaternion.Lerp(body.localRotation, Quaternion.Euler(bodyTarget), .1f);
    }

    void UpdateWheel(WheelCollider collider, Transform wheel)
    {
        bool notDrifting = !isDrifting && Mathf.Abs(allWheels.localEulerAngles.y) <= .05f;
        if (notDrifting)
        {
            collider.GetWorldPose(out Vector3 pos, out Quaternion quat);
            wheel.SetPositionAndRotation(pos, quat);
        }
        wheelRotationOffset += transform.InverseTransformDirection(car.CRigidbody.velocity).z * Time.deltaTime * Mathf.PI * 2;
        wheel.localEulerAngles += new Vector3(wheelRotationOffset, 0);
        
    }

    public bool IsGrounded()
    {
        return (fr.isGrounded && fl.isGrounded && rr.isGrounded && rl.isGrounded);
    }

    public bool IsGroundedAny()
    {
        return (fr.isGrounded || fl.isGrounded || rr.isGrounded || rl.isGrounded);
    }

    private void FixedUpdate()
    {
        if (controlable)
        {
            temp_vel = car.CRigidbody.velocity.magnitude;
            Vector3 temp_up = Vector3.up;
            localVelocity = transform.InverseTransformDirection(car.CRigidbody.velocity);

            // Calculate acceleration
            float cc = Mathf.Pow(.5f, 1f + maxSpeed / 120f); // set to .25f for max 120, .175f for max 160, .125f for max 240
            if (IsGroundedAny())
            {
                car.CRigidbody.velocity += (3 + Mathf.Log((temp_vel * cc) + 1, .5f)) / 64 * acceleration * a_vertical * transform.forward * Time.deltaTime * accelRate;
            }

            Steer();

            // Antigravity and downforce
            if (isOnAntigrav)
            {
                if (IsGroundedAny())
                {
                    temp_up = transform.up;
                    car.CRigidbody.AddForce(-2000 * downforce * temp_up);
                }
            }
            else
            {
                temp_up = Vector3.up;
            }
            car.CRigidbody.AddForce(-1f * downforce * temp_vel * transform.up);

            // Brake, when pressing the key that's opposite to the direction of movement

            if (localVelocity.z * a_vertical < 0 && IsGroundedAny()) 
            { 
                car.CRigidbody.velocity += brakeTorque * localVelocity.normalized.z * Time.deltaTime * -transform.forward; 
            }

            // Reverse
            if (localVelocity.z > -.25f) reversing = false;
            else if (a_vertical == -1) reversing = true;

            // If no keys in vertical axis are pressed, slowly deccelerate
            if (a_vertical == 0) car.CRigidbody.velocity *= idleDecceleration;

            // Apply custom gravity
            car.CRigidbody.AddForce(-9.81f * car.CRigidbody.mass * temp_up);
            
            // Drift
            if (IsGroundedAny())
            {
                if (a_drift == 0)
                {
                    driftDirection = a_horizontal;
                    isDrifting = false;
                }
                else
                {
                    if (localVelocity.z >= 10) isDrifting = true;
                    else isDrifting = false;
                }
            }
            else isDrifting = false;

            // If below y=0, reset
            if (transform.position.y < 0 && !isResetting)
            {
                isResetting = true;
                StartCoroutine(nameof(QueueResetAfterFall));
            }

        }

    }

    IEnumerator QueueResetAfterFall()
    {
        yield return new WaitForSeconds(2);
        ResetCar();
        isResetting = false;
    }

    void Steer()
    {
        if (a_horizontal != 0) h = Mathf.MoveTowards(h, a_horizontal, Time.deltaTime * (1 / turnSmoothTime));
        else h = 0;
        float i = car.CRigidbody.velocity.magnitude / 50;
        fr.steerAngle = (Mathf.LerpUnclamped(maxSteerAngle, minSteerAngle, i)) * h * 2;
        fl.steerAngle = (Mathf.LerpUnclamped(maxSteerAngle, minSteerAngle, i)) * h * 2;
        if (isDrifting)
        {
            fr.steerAngle += maxSteerAngle * driftDirection * 1.25f;
            fl.steerAngle += maxSteerAngle * driftDirection * 1.25f;
        }
    }

    public void ResetCar(bool fullReset = false)
    {
        if (controlable)
        {
            if (!FindObjectOfType<Pause>().paused)
            {
                if (isInPractice) 
                { 
                    CheckpointResetCar(); 
                    Debug.Log("isInPractice = true"); 
                }
                else
                {
                    if (car.CRigidbody != null) // this is a workaround to a problem with new input system
                    {
                        car.CRigidbody.velocity = Vector3.zero;
                        car.CRigidbody.angularVelocity = Vector3.zero;
                    }
                    engineSound.pitch = 1f;
                    if (car.gameMode == Constants.GameMode.TimeAttack || fullReset)
                    {
                        isOnAntigrav = false;
                        if (car.CRigidbody != null) car.CRigidbody.isKinematic = true;
                        controlable = false;
                        engineSound.pitch = 1;
                        carLoader.StartCoroutine(carLoader.ResetCountDown());
                        transform.SetPositionAndRotation(startPosition, startRotation);
                    }
                    else
                    {
                        transform.SetPositionAndRotation(respawnPosition, respawnRotation);
                    }
                }
            }
        }
    }

    //Practice Mode Code
    private void CheckpointResetCar()
    {
        Transform _Checkpoint = transform;
        Rigidbody _Vels = car.CRigidbody;
        if (hasCustomCheckpoint)
        {
            _Checkpoint.SetPositionAndRotation(customCheckpointPosition, customCheckpointRotation);
            _Vels.velocity = savedVelocityCustom;
            _Vels.angularVelocity = savedAngularVelocityCustom;
        }
        else
        {
            _Checkpoint.SetPositionAndRotation(levelCheckpointPosition, levelCheckpointRotation);
            _Vels.velocity = savedVelocityLevel;
            _Vels.angularVelocity = savedAngularVelocityLevel;
            if (currentCheckpoint == 0)
            {
                _Checkpoint.SetPositionAndRotation(respawnPosition, respawnRotation);
            }
        }
        transform.SetPositionAndRotation(_Checkpoint.position, _Checkpoint.rotation);
        fr.motorTorque = motorTorques[0];
        fl.motorTorque = motorTorques[1];
        rr.motorTorque = motorTorques[2];
        rl.motorTorque = motorTorques[3];
        car.CRigidbody.velocity = _Vels.velocity;
        car.CRigidbody.angularVelocity = _Vels.angularVelocity;
        if (!car.IsAI) car.CTimer.StartCoroutine(car.CTimer.SetSavedTimes(savedTime));
    }
    //Practice mode
    public void createCheckpoint(bool isCustom = false)
    {
        if (controlable && isInPractice)
        {
            currentCheckpoint = car.CPosition.checkpoint;
            savedTime = car.CTimer.GetCurrentTime();
            if (isCustom)
            {
                hasCustomCheckpoint = true;
                customCheckpointPosition = transform.position;
                customCheckpointRotation = transform.rotation;
                savedVelocityCustom = car.CRigidbody.velocity;
                savedAngularVelocityCustom = car.CRigidbody.angularVelocity;
            }
            else
            {
                hasCustomCheckpoint = false;
                levelCheckpointPosition = transform.position;
                levelCheckpointRotation = transform.rotation;
                savedVelocityLevel = car.CRigidbody.velocity;
                savedAngularVelocityLevel = car.CRigidbody.angularVelocity;
            }
            motorTorques[0] = fr.motorTorque;
            motorTorques[1] = fl.motorTorque;
            motorTorques[2] = rr.motorTorque;
            motorTorques[3] = rl.motorTorque;
        }
    }
    private void removeLastCustomCheckpoint()
    {
        if (controlable && isInPractice) hasCustomCheckpoint = false;
    }
}
