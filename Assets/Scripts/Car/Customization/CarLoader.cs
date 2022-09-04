using UnityEngine;
using System;

public class CarLoader : MonoBehaviour
{
    public GameObject[] cars;
    public CarStats[] stats;

    public GameObject GetCarPrefab(int index, int engine, bool isInMenu, bool isAI)
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
        CarStats s;
        try
        {
            s = stats[engine];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("Engine not defined. Loading default engine stats");
            s = stats[0];
        }
        car.GetComponent<CarBrain>().Init(isAI, isInMenu, s);
        
        car.name = isAI ? "AI Car" : "Car";
        return car;
    }
}
