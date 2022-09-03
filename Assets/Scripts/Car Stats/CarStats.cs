using UnityEngine;

[CreateAssetMenu(fileName = "Car Stats", menuName = "Car Stats")]
public class CarStats : ScriptableObject
{
    public float maxSpeed, acceleration, minSteerAngle, maxSteerAngle, brakeTorque, idleDecceleration, downforce;
}
