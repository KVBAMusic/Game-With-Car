using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Vector3 rotation;
    public float speed;

    void Update()
    {
        transform.Rotate(rotation * speed * Time.deltaTime);
    }
}
