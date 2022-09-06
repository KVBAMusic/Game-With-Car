using System;
using UnityEngine;

public class CarLapTracker : MonoBehaviour
{
    CarBrain car;

    public event EventHandler OnRaceStart;
    public event EventHandler OnLapStarted;
    public event EventHandler OnCheckpointReached;
    public event EventHandler OnFinalLapStarted;
    public event EventHandler OnRaceEnd;

    public int CurrentLap {get; private set;}
    public int Checkpoint {get; private set;}
    public int CurrentPath {get; private set;}

    void Awake()
    {
        car = GetComponent<CarBrain>();
        car.OnReset += Reset;
        Init();
    }

    void Init()
    {
        CurrentLap = 1;
        CurrentPath = 1;
        Checkpoint = 0;
    }

    public void StartRace() 
    {
        Checkpoint = 1;
        OnRaceStart?.Invoke(this, EventArgs.Empty);
    }

    void NextLap()
    {
        Checkpoint = 1;
        CurrentLap++;
        CurrentPath = 1;
        car.ResetPath();

        OnLapStarted?.Invoke(this, EventArgs.Empty);
        if (car.CPosition.lap == car.NumLaps)
        {
            OnFinalLapStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    void EndRace() 
    {
        Checkpoint = 4;
        OnRaceEnd?.Invoke(this, EventArgs.Empty);
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Start":
                if (car.CPosition.checkpoint == 3)
                {
                    if (car.CPosition.lap == car.NumLaps)
                    {
                        EndRace();
                    }
                    else NextLap();
                }
                break;
            case "CP1":
                if (Checkpoint == 1)
                {
                    Checkpoint = 2;
                    OnCheckpointReached?.Invoke(this, EventArgs.Empty);
                }
                break;
            case "CP2":
                if (Checkpoint == 2)
                {
                    Checkpoint = 3;
                    OnCheckpointReached?.Invoke(this, EventArgs.Empty);
                }
                break;
            default:
                break;
        }
    }

    void Reset(object sender, EventArgs e)
    {
        Init();
    }
}