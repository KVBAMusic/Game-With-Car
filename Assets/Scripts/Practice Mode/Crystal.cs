using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Material custom;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 50));
    }
    private void Start()
    {
        GetComponent<MeshRenderer>().material = custom;
    }
}
