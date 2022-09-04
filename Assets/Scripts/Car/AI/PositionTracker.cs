using System;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour, ICarComponent
{
    CarBrain car;
    public int lap; 
    public int checkpoint; 
    public int currentPath;
    public int pointOnPath;
    public float distance;

    public Vector3 WorldPointOnPath => car.Path.GetPoint(pointOnPath);

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
        currentPath = 1;
        pointOnPath = 0;
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
        var currentPoint = car.Path.GetPoint(pointOnPath);
        distance = (currentPoint - transform.position).magnitude;
        if (distance < 40)
        {
            int nextIdx = (pointOnPath + 1) % car.Path.NumPoints;
            if (nextIdx > pointOnPath)
            {
                pointOnPath = nextIdx;
            }
        }
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
