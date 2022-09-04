using UnityEngine;

public class CheckpointSwitch : MonoBehaviour
{
    public void SwitchPractice(bool value)
    {
        // Use only in time attack
        // Might change it in future
        if (PlayerPrefs.HasKey("Game Mode") && (Constants.GameMode)PlayerPrefs.GetInt("Game Mode") == Constants.GameMode.TimeAttack)
        {
            CarBrain Car = GameObject.FindGameObjectWithTag("Car").GetComponent<CarBrain>();
            Car.CMovement.isInPractice = value;
            Car.ResetCar();
            if (value == true) Car.CMovement.createCheckpoint();
            else Car.CTimer.practiceOffsetTime = 0;
        }
    }
}
