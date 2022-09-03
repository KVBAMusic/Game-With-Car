using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointNumber;
    void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    public int GetCheckpointNumber()
    {
        return checkpointNumber;
    }
}
