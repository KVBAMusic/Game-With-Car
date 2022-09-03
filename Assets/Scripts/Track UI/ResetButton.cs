using UnityEngine;

public class ResetButton : MonoBehaviour
{

    GameObject[] cars;
    private void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
    }

    public void ResetCar()
    {
        foreach (var car in cars)
        {
            var pm = car.GetComponent<PlayerMovement>();
            pm.ResetCar(true);
            if (pm.isAI)
            {
                car.GetComponent<BetterCarAIController>().ResetCar();
            }
            car.GetComponent<CVATrigger>().ResetCar();

        }
    }
}
