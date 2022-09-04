using System;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour, ICarComponent
{
    CarBrain car;
    public int lap, checkpoint, botCheckpoint;
    public float distance;

    public int place { get;  set; }
    // Start is called before the first frame update
    void Awake()
    {
        car = GetComponent<CarBrain>();
        car.OnReset += Reset;
        car.CLapTracker.OnRaceStart += StartRace;
        car.CLapTracker.OnLapStarted += NewLap;
        car.CLapTracker.OnRaceEnd += EndRace;
        car.CLapTracker.OnCheckpointReached += CheckpointReached;
    }

    void Start()
    {
        PrepareForRace();
    }

    void PrepareForRace()
    {
        lap = 1;
        checkpoint = 0;
        botCheckpoint = 0;
    }

    public void StartRace(object sender, EventArgs e)
    {
        checkpoint = 1;
    }

    public void EndRace(object sender, EventArgs e)
    {
        checkpoint = 4;
    }

    // Update is called once per frame
    void Update()
    {
        lap = car.CPosition.lap;
        checkpoint = car.CPosition.checkpoint;
        botCheckpoint = car.CMovement.botCheckpoint;
        int nextCheckpoint = (botCheckpoint + 1) % car.CMovement.positionCheckpoints.Length;
        distance = Vector3.Distance(transform.position, car.CMovement.positionCheckpoints[nextCheckpoint].checkpoint.position);
    }

    public void Reset(object sender, EventArgs e)
    {
        PrepareForRace();
    }

    public void NewLap(object sender, EventArgs e)
    {
        lap++;
        checkpoint = 1;
    }

    public void CheckpointReached(object sender, EventArgs e)
    {
        checkpoint++;
    }
}
