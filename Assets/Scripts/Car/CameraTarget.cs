using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    Transform car;
    PlayerMovement pm;
    public float smooth;
    public GameObject carBody;
    [HideInInspector] public Quaternion targetRotation;
    Rigidbody carRb;
    Vector3 temp_targetRotation;
    Vector3 carUp;
    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("Car(Clone)").transform;
        carRb = car.gameObject.GetComponent<Rigidbody>();
        pm = car.gameObject.GetComponent<PlayerMovement>();
        transform.rotation = car.rotation;
        targetRotation = car.rotation;
        carUp = car.up;
    }

    private void Update()
    {

        if (pm.IsGroundedAny())
        {
            targetRotation = car.rotation;
            carUp = Vector3.Lerp(carUp, car.up, smooth * Time.deltaTime);
        }                                                         // Follow the car if on ground, and update carUp
        else                                                      // Else, freeze the rotation in all axes but Y
        {                                                         // This way, the player can see what's in front of the car when they're falling
            temp_targetRotation = transform.eulerAngles;
            temp_targetRotation.x = -carRb.velocity.y * .75f;
            targetRotation = Quaternion.Euler(temp_targetRotation);
        }
        transform.position = car.position + carUp * 3;           // Apply transform so that the target appears above the car, not under it
        if (!pm.isOnAntigrav)                                    // If it's not on antigrav surface, don't roll the camera
        {
            temp_targetRotation = targetRotation.eulerAngles;
            temp_targetRotation.z = 0;
            targetRotation = Quaternion.Euler(temp_targetRotation);
        }
        if (pm.reversing)                                        // If it's reversing, flip the camera
        {
            temp_targetRotation = targetRotation.eulerAngles;
            temp_targetRotation.x *= -1;
            temp_targetRotation.y += 180;
            temp_targetRotation.z *= -1;
            if (pm.IsGroundedAny()) targetRotation = Quaternion.Euler(temp_targetRotation);
        }
        if (car.position.y < 0)                                  // Don't follow the car if it's below Y = 0
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            transform.LookAt(car);
        }
        // Apply smoothing to the rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smooth * Time.deltaTime);
    }


}
