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

    void Awake()
    {
        car = GetComponent<CarBrain>();
    }

    public void StartRace() =>
        OnRaceStart?.Invoke(this, EventArgs.Empty);

    void NextLap()
    {
        OnLapStarted?.Invoke(this, EventArgs.Empty);
        car.ResetPath();
        if (car.CPosition.lap == car.NumLaps)
        {
            OnFinalLapStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    void EndRace() =>
        OnRaceEnd?.Invoke(this, EventArgs.Empty);

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{car.CPosition.checkpoint}\t{car.CPosition.lap}\t{car.NumLaps}");
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
                OnCheckpointReached?.Invoke(this, EventArgs.Empty);
                break;
            case "CP2":
                OnCheckpointReached?.Invoke(this, EventArgs.Empty);
                break;
            default:
                break;
        }
    }
}