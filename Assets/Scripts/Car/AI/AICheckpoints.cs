using UnityEngine;

public class AICheckpoints : MonoBehaviour
{
    public AICheckpoint[] checkpoints;
    public float turnThreshold, distanceThreshold, detectionDistance = 10;
}

[System.Serializable]
public class AICheckpoint
{
    public Transform checkpoint;
    public bool needsDesiredSpeed = false;
    public float desiredSpeed;
    public bool ignoreDistanceThreshold = false;
}
