using UnityEngine;
using System;

public interface IActionGate
{
    public GameObject g { get; set; }
    public event EventHandler OnGateHit;
    public void Action(GameObject g);
}
