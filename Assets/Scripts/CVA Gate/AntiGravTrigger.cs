using UnityEngine;
using System;

public class AntiGravTrigger : MonoBehaviour, IActionGate
{
    [SerializeField] GameObject vfx;
    public GameObject g { get; set; }
    public event EventHandler OnGateHit;

    public bool activate;

    public void Action(GameObject g)
    {
        this.g = g;
        OnGateHit += AntiGravTrigger_OnGateHit;
        OnGateHit?.Invoke(this, EventArgs.Empty);
    }

    private void AntiGravTrigger_OnGateHit(object sender, EventArgs e)
    {
        g.GetComponent<CVATrigger>().AGGateHit(activate, vfx);
        OnGateHit = null;
    }
}
