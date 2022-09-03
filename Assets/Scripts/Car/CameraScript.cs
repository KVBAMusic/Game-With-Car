using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Transform car;
    public Camera mainCamera, frontcamera;
    public int cameras;
    int activeCamera = 0;
    Transform frontCamTransform;

    InputMain input;


    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        car = GameObject.Find("Car(Clone)").transform;
        frontCamTransform = frontcamera.GetComponent<Transform>();
        input = new InputMain();
        input.Enable();
        input.Player.ChangeCamera.performed += ctx => ChangeCamera();
    }

    // Update is called once per frame
    void Update()
    {
        frontCamTransform.position = car.position + (car.forward * 2.7f) + car.up * 1.5f;
        frontCamTransform.rotation = car.rotation;
        UpdateCamera();
    }

    void ChangeCamera()
    {
        activeCamera++;
        if (activeCamera >= cameras)
        {
            activeCamera = 0;
        }
    }

    private void UpdateCamera()
    {
        mainCamera.gameObject.SetActive(activeCamera == 0);
        frontcamera.gameObject.SetActive(activeCamera == 1);
    }
}
