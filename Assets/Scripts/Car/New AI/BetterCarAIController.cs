using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Based on tutorial by EYEmaginary
/// https://www.youtube.com/playlist?list=PLB9LefPJI-5wH5VdLFPkWfnPjeI6OSys1
/// </summary>
public class BetterCarAIController : MonoBehaviour
{
    private const bool debug = true;

    [HideInInspector] public Transform path;
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

    private List<Transform> nodes = new();
    private int currentNode = 0;
    readonly int trackLayerMask = 1 << 7;
    readonly int fullLayerMask = (1 << 6) + (1 << 7);
    Rigidbody rb;

    float steerAmount;
    float driveAmount;
    float brakingZoneMaxSpeed = 0;
    float localVelocity;
    bool frontStuck = false;
    bool backStuck = false;

    private void Awake()
    {
        try 
        { 
            path = GameObject.Find("Path").transform;
        }
        catch(NullReferenceException)
        {
            path = null;
        }
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Get all the nodes in the path
        if (path != null)
        {
            Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
            nodes = new();

            for (int i = 0; i < pathTransforms.Length; i++)
            {
                if (pathTransforms[i] != path.transform)
                {
                    nodes.Add(pathTransforms[i]);
                }
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        if (path != null)
        {
            DebugLines(); 
        }
    }

    private void FixedUpdate()
    {
        if (path != null)
        {
            localVelocity = transform.InverseTransformDirection(rb.velocity).z;
            steerAmount = ApplySteer();
            driveAmount = Drive();
            steerAmount = Mathf.Clamp(steerAmount + Sensors(), -1, 1);
            CheckWaypoints();
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
        Gizmos.DrawLine(transform.position, nodes[currentNode].position);
    }

    float ApplySteer()
    {
        Vector3 relativeVector = (transform.InverseTransformPoint(nodes[currentNode].position)).normalized;
        return Mathf.Clamp(relativeVector.x * turnSmoothingAmount, -1, 1);
    }

    float Drive()
    {
        float output = 0;
        Vector3 dir = (nodes[currentNode].position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dir);
        output = dot >= dotProductReverseThreshold ? 1 : -1;
        if (Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z) > brakingZoneMaxSpeed  / 3.6f && brakingZoneMaxSpeed > 0)
        {
            output *= -1;
        }
        return output;
    }

    void CheckWaypoints()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < waypointDistanceThreshold)
        {
            currentNode = (currentNode + 1) % nodes.Count;
        }
    }

    public void ResetCar()
    {
        currentNode = 0;
    }
}