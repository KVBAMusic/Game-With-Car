using System;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

/// <summary>
/// Based on tutorial by EYEmaginary
/// https://www.youtube.com/playlist?list=PLB9LefPJI-5wH5VdLFPkWfnPjeI6OSys1
/// </summary>
public class BetterCarAIController : MonoBehaviour
{
    CarBrain car;
    private const bool debug = true;

    [Header("Navigation")]
    public float waypointDistanceThreshold = 0.1f;
    public float dotProductReverseThreshold = 0;
    public float turnSmoothingAmount = 3;


    [Header("Sensors")]
    public float sensorDistanceThreshold = 5;
    public float sensorDistanceOffset = 2;
    public float sensorDistanceSideOffset = 1;
    public float sensorSideAngle = 30;
    [HideInInspector] public PlayerMovement pm;

    readonly int trackLayerMask = 1 << 7;
    readonly int fullLayerMask = (1 << 6) + (1 << 7);

    float steerAmount;
    float driveAmount;
    float brakingZoneMaxSpeed = 0;
    float localVelocity;
    bool frontStuck = false;
    bool backStuck = false;

    private void Awake()
    {
        car = GetComponent<CarBrain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Braking Zone"))
        {
            brakingZoneMaxSpeed = other.GetComponent<BrakingZone>().speed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Braking Zone"))
        {
            brakingZoneMaxSpeed = 0;
        }
    }

    private void FixedUpdate()
    {
        if (car.Path != null)
        {
            localVelocity = transform.InverseTransformDirection(car.CRigidbody.velocity).z;
            steerAmount = ApplySteer();
            driveAmount = Drive();
            steerAmount = Mathf.Clamp(steerAmount + Sensors(), -1, 1);
            pm.SetAxisAI(driveAmount, steerAmount);
        }
    }

    float Sensors()
    {
        float output = 0;
        RaycastHit hit;
        Vector3 sensorStartingPos = transform.position + transform.forward * sensorDistanceOffset + transform.up * .5f;
        Vector3 sensorStartingBackPos = transform.position - transform.forward * sensorDistanceOffset + transform.up * .5f;

        // facing front, right
        if (Physics.Raycast(sensorStartingPos + transform.right * sensorDistanceSideOffset, transform.forward, out hit, sensorDistanceThreshold, fullLayerMask))
        {
            output -= .5f;
        }
        // facing front, left
        if (Physics.Raycast(sensorStartingPos - transform.right * sensorDistanceSideOffset, transform.forward, out hit, sensorDistanceThreshold, fullLayerMask))
        {
            output += .5f;
        }

        // angled right
        if (Physics.Raycast(sensorStartingPos + transform.right * sensorDistanceSideOffset, Quaternion.AngleAxis(sensorSideAngle, transform.up) * transform.forward, out hit, sensorDistanceThreshold, fullLayerMask))
        {
            output -= .5f;
        }

        // angled left
        if (Physics.Raycast(sensorStartingPos - transform.right * sensorDistanceSideOffset, Quaternion.AngleAxis(-sensorSideAngle, transform.up) * transform.forward, out hit, sensorDistanceThreshold, fullLayerMask))
        {
            output += .5f;
        }

        // facing rear, middle
        if (Physics.Raycast(sensorStartingBackPos, -transform.forward, out hit, sensorDistanceThreshold * 3f, trackLayerMask))
        {
            if (Mathf.Abs(localVelocity) < 10)
            {
                backStuck = true;
                if (hit.normal.x > 0)
                {
                    output = 1;
                }
                else
                {
                    output = -1;
                }
                driveAmount = 1;
            }
        }
        else
        {
            backStuck = false;
        }

        // facing front, middle
        if (Physics.Raycast(sensorStartingPos, transform.forward, out hit, sensorDistanceThreshold, trackLayerMask))
        {
            if (hit.normal.x > 0)
            {
                output = -1;
            }
            else
            {
                output = 1;
            }
            if (Mathf.Abs(localVelocity) < 18)
            {
                if (Physics.Raycast(sensorStartingPos, transform.forward, out hit, sensorDistanceThreshold * 4f, fullLayerMask))
                {
                    frontStuck = true;
                }
                else
                {
                    frontStuck = false;
                }
                if (frontStuck)
                {
                    output *= -1;
                    driveAmount = -1;
                }
            }
        }
        
        return output;
    }

    void DebugLines()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, car.CPosition.WorldPointOnPath);
    }

    float ApplySteer()
    {
        Vector3 relativeVector = (transform.InverseTransformPoint(car.CPosition.WorldPointOnPath)).normalized;
        return Mathf.Clamp(relativeVector.x * turnSmoothingAmount, -1, 1);
    }

    float Drive()
    {
        float output = 0;
        Vector3 dir = (car.CPosition.WorldPointOnPath - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);
        output = dot >= dotProductReverseThreshold ? 1 : -1;
        if (Mathf.Abs(transform.InverseTransformDirection(car.CRigidbody.velocity).z) > brakingZoneMaxSpeed  / 3.6f && brakingZoneMaxSpeed > 0)
        {
            output *= -1;
        }
        return output;
    }
}