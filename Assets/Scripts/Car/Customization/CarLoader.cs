using UnityEngine;
using System;

public class CarLoader : MonoBehaviour
{
    public GameObject[] cars;
    public CarStats[] stats;

    public GameObject GetCarPrefab(int index, int engine, bool isInMenu, bool isAI = false)
    {
        GameObject car;
        try
        {
            car = cars[index];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("Car not defined. Loading default car");
            car = cars[0];
        }
        PlayerMovement pm = car.GetComponent<PlayerMovement>();
        try
        {
            pm.stats = stats[engine];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("Engine not defined. Loading default engine stats");
            pm.stats = stats[0];
        }
        if (isInMenu)
        {
            car.GetComponent<PlayerMovement>().controlable = false;
            car.GetComponent<SpeedIndicator>().enabled = false;
            car.GetComponent<CVATrigger>().enabled = false;
            car.GetComponent<Rigidbody>().isKinematic = false;
            car.GetComponent<Rigidbody>().useGravity = true;
            car.GetComponent<AudioSource>().enabled = false;
            car.GetComponent<PositionTracker>().enabled = false;
            car.GetComponent<TimerCheckpoints>().enabled = false;
        }
        else
        {
            car.GetComponent<PlayerMovement>().controlable = false;
            car.GetComponent<Rigidbody>().useGravity = false;
            car.GetComponent<CVATrigger>().enabled = true;
            car.GetComponent<Rigidbody>().isKinematic = true;
            car.GetComponent<AudioSource>().enabled = true;
            car.GetComponent<PositionTracker>().enabled = true;
            car.GetComponent<TimerCheckpoints>().enabled = true;
            if (isAI)
            {
                car.GetComponent<BetterCarAIController>().enabled = true;
                car.GetComponent<PlayerMovement>().isAI = true;
                car.GetComponent<TimerCheckpoints>().isAI = true;
                car.GetComponent<SpeedIndicator>().enabled = false;
            }
            else
            {
                car.GetComponent<BetterCarAIController>().enabled = false;
                car.GetComponent<PlayerMovement>().isAI = false;
                car.GetComponent<TimerCheckpoints>().isAI = false;
                car.GetComponent<SpeedIndicator>().enabled = true;
            }
        }
        car.name = isAI ? "AI Car" : "Car";
        return car;
    }
}
