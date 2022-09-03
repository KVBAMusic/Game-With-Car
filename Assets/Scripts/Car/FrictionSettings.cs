using System;
using UnityEngine;

public class FrictionSettings : MonoBehaviour
{
    public FrictionSetting[] settings;
}

[System.Serializable]
public class FrictionSetting
{
    public string name;
    public float accelerationRate;
    public float[] forwardValues = { 0, 0, 0, 0, 0 };
    public float[] sidewaysValues = { 0, 0, 0, 0, 0 };

    public FrictionSetting()
    {
        return;
    }

    public WheelFrictionCurve GetFrictionCurve(int c)
    {
        WheelFrictionCurve wfc = new();

        if (c == 0)
        {
            wfc.asymptoteSlip = forwardValues[2];
            wfc.asymptoteValue = forwardValues[3];
            wfc.extremumSlip = forwardValues[0];
            wfc.extremumValue = forwardValues[1];
            wfc.stiffness = forwardValues[4];
        }
        else
        {
            wfc.asymptoteSlip = sidewaysValues[2];
            wfc.asymptoteValue = sidewaysValues[3];
            wfc.extremumSlip = sidewaysValues[0];
            wfc.extremumValue = sidewaysValues[1];
            wfc.stiffness = sidewaysValues[4];
        }
        return wfc;
    }
}

