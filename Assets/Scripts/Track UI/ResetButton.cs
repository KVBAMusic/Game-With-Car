using UnityEngine;

public class ResetButton : MonoBehaviour
{
    CarBrain[] cars;
    private void Start()
    {
        cars = FindObjectsOfType<CarBrain>();
    }

    public void ResetCar()
    {
        foreach (var car in cars)
        {
            car.ResetCar();
        }
    }
}
