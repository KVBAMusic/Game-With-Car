using UnityEngine;
using System;

public interface ICarComponent
{
    public void StartRace(object sender, EventArgs e);
    public void NewLap(object sender, EventArgs e);
    public void CheckpointReached(object sender, EventArgs e);
    public void EndRace(object sender, EventArgs e);
    public void Reset(object sender, EventArgs e);
}