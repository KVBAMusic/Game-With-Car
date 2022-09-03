using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public int lap, checkpoint, botCheckpoint;
    public float distance;

    public int place { get;  set; }

    PlayerMovement pm;
    TimerCheckpoints tc;
    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        tc = GetComponent<TimerCheckpoints>();
    }

    // Update is called once per frame
    void Update()
    {
        lap = tc.currentLap;
        checkpoint = tc.checkpoint;
        botCheckpoint = pm.botCheckpoint;
        int nextCheckpoint = (botCheckpoint + 1) % pm.positionCheckpoints.Length;
        distance = Vector3.Distance(transform.position, pm.positionCheckpoints[nextCheckpoint].checkpoint.position);
    }

    public void ResetCar()
    {
        lap = 0;
        checkpoint = 0;
        botCheckpoint = 0;
        distance = 0;
    }
}
