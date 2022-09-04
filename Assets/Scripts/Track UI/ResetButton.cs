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
            var pm = car.CMovement;
            pm.ResetCar(true);
            if (car.IsAI)
            {
                car.CAIController.ResetCar();
            }
            car.CGateTrigger.ResetCar();

        }
    }
}
