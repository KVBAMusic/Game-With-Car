using UnityEngine;
using System;

public class BoostGateTrigger : MonoBehaviour, IActionGate
{
    public Transform direction;
    Vector3 euler;
    public GameObject g { get; set; }

    public event EventHandler OnGateHit;

    void Start()
    {
        euler = direction.eulerAngles;
    }    

    public void Action(GameObject g)
    {
        this.g = g;
        OnGateHit += CVADirection_OnGateHit;
        OnGateHit?.Invoke(this, EventArgs.Empty);
    }

    private void CVADirection_OnGateHit(object sender, EventArgs e)
    {
        g.GetComponent<CVATrigger>().BoostGateHit(direction, euler);
        OnGateHit = null;
    }
}
