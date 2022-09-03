using UnityEngine;

public class CheckpointSwitch : MonoBehaviour
{
    public void SwitchPractice(bool value)
    {
        // Use only in time attack
        // Might change it in future
        if (PlayerPrefs.HasKey("Game Mode") && (Constants.GameMode)PlayerPrefs.GetInt("Game Mode") == Constants.GameMode.TimeAttack)
        {
            PlayerMovement Car = GameObject.FindGameObjectWithTag("Car").GetComponent<PlayerMovement>();
            TimerCheckpoints tc = GameObject.FindGameObjectWithTag("Car").GetComponent<TimerCheckpoints>();
            Car.isInPractice = value;
            Car.ResetCar();
            if (value == true) Car.createCheckpoint();
            else tc.practiceOffsetTime = 0;
        }
    }
}
