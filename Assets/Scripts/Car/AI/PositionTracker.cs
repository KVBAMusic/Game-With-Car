using System;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    CarBrain car;
    public int lap; 
    public int checkpoint; 
    public int currentPath;
    public int pointOnPath;
    public float distance;

    public Vector3 WorldPointOnPath => car.Path.GetPoint(pointOnPath);

    public int place { get;  set; }
    public int LockedPlace {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        car = GetComponent<CarBrain>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        lap = car.CLapTracker.CurrentLap;
        checkpoint = car.CLapTracker.Checkpoint;
        currentPath = car.CLapTracker.CurrnetPath;
        pointOnPath = car.CLapTracker.PointOnPath;

        if (checkpoint != 4) 
            LockedPlace = place;

        distance = (WorldPointOnPath - transform.position).magnitude;
        if (distance <= 40)
        {
            int nextIdx = (pointOnPath + 1) % car.Path.NumPoints;
            if (nextIdx > pointOnPath)
            {
                pointOnPath = nextIdx;
            }
        }
    }
}
