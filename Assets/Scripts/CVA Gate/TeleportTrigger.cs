using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour, IActionGate
{
    public GameObject g { get; set; }
    public event EventHandler OnGateHit;

    public Transform dest;

    public void Action(GameObject g)
    {
        this.g = g;
        OnGateHit += TeleportTrigger_OnGateHit;
        OnGateHit?.Invoke(this, EventArgs.Empty);
    }

    private void TeleportTrigger_OnGateHit(object sender, EventArgs e)
    {
        g.GetComponent<CVATrigger>().TeleportGateHit(dest);
        OnGateHit = null;
    }
}
