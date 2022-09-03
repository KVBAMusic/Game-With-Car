using System;
using UnityEngine;

public class WheelPhysics : MonoBehaviour
{
    public WheelCollider wc;
    public PlayerMovement pl;
    public float driftAmount;
    [SerializeField] FrictionSettings fs;
    void Start()
    {
        wc = GetComponent<WheelCollider>();
    }

    private void FixedUpdate()
    {
        WheelHit wh = new();
        wc.GetGroundHit(out wh);
        if (wh.collider == null)
        {
            return;
        }
        else
        {
            try
            {
                FrictionSetting setting = new FrictionSetting();
                for (int i = 0; i < fs.settings.Length; i++)
                {
                    if (wh.collider.gameObject.CompareTag(fs.settings[i].name))
                    {
                        setting = fs.settings[i];
                        break;
                    }
                }
                wc.forwardFriction = setting.GetFrictionCurve(0);
                wc.sidewaysFriction = setting.GetFrictionCurve(1);
                pl.accelRate = setting.accelerationRate;
            }
            catch (NullReferenceException)
            {
                return;
            }
        }
    }

}