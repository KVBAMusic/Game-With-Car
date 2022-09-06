using System;
using UnityEngine;

public class CarUIController : MonoBehaviour
{
    CarBrain car;
    public TimerUIUpdate timerUI;

    Pause pause;

    void Awake()
    {
        car = GetComponent<CarBrain>();

        timerUI = FindObjectOfType<TimerUIUpdate>();
        pause = FindObjectOfType<Pause>();
    }

    void Start()
    {
        if (car.gameMode == Constants.GameMode.TimeAttack)
        {
            timerUI.SetActive(true, ref timerUI.timeAttackUI);
            timerUI.SetActive(false, ref timerUI.raceUI);
            timerUI.SetActive(false, ref timerUI.timeDiffS1);
            timerUI.SetActive(false, ref timerUI.timeDiffS2);
            timerUI.SetActive(false, ref timerUI.timeDiffS3);
            timerUI.UpdateText("00:00.00", ref timerUI.time);
            timerUI.UpdateText("", ref timerUI.bestTime);
            if (PlayerPrefs.HasKey("username"))
            {
                timerUI.SetActive(false, ref timerUI.leaderboardStatus);
            }
        }
        else
        {
            timerUI.SetActive(false, ref timerUI.timeAttackUI);
            timerUI.SetActive(true, ref timerUI.raceUI);
            timerUI.UpdateText("-/-", ref timerUI.time);
            timerUI.SetActive(false, ref timerUI.finalLap);
            timerUI.UpdateText($"Current lap\n{Mathf.Clamp(car.CPosition.lap, 1, car.NumLaps)}/{car.NumLaps}", ref timerUI.currentLap);
        }
    }

    private void Update()
    {
        if (!car.IsAI)
        {
            if (car.CPosition.checkpoint != 4)
            {
                timerUI.UpdateText(Constants.FormatTime(car.CTimer.GetCurrentTime()), ref timerUI.time);
                timerUI.UpdateText($"Current lap\n{Mathf.Clamp(car.CPosition.lap, 1, car.NumLaps)}/{car.NumLaps}", ref timerUI.currentLap);
                timerUI.UpdateText($"{car.CPosition.place}/{PlayerPrefs.GetInt("Bots") + 1}", ref timerUI.position);
            }
            Cursor.visible = (car.CPosition.checkpoint == 4 || pause.paused);
        }
    }

    void OnFinalLap(object sender, EventArgs e)
    {
        if (!car.IsAI) timerUI.SetActiveForTime(3, ref timerUI.finalLap);
    }

    public void StartRace()
    {
        if (!car.IsAI)
        {
            timerUI.UpdateTextColour(Color.yellow, ref timerUI.time);
            timerUI.UpdateTextColour(Color.yellow, ref timerUI.position);
        }
    }

    public void DisplaySectorTime()
    {
        int cp = car.CPosition.checkpoint;
        var textObject = cp switch
        {
            1 => timerUI.timeS1,
            2 => timerUI.timeS2,
            3 => timerUI.timeS3,
            _ => null
        };
        float sectorTime = cp switch
        {
            1 => car.CTimer.sector1Time,
            2 => car.CTimer.sector2Time,
            3 => car.CTimer.sector3Time,
            _ => -1f
        };
        timerUI.UpdateText($"Sector {cp} {Constants.FormatTime(sectorTime)}", ref textObject);
    }

    public void DisplaySectorDifference(float difference)
    {
        int cp = car.CPosition.checkpoint;
        var textDiffObject = cp switch
        {
            1 => timerUI.timeDiffS1,
            2 => timerUI.timeDiffS2,
            3 => timerUI.timeDiffS3,
            _ => null
        };
        Color textColor = Color.white;
        char mod = ' ';
        if (difference > 0) { textColor = Color.red; mod = '+';}
        if (difference < 0) { textColor = Color.green; mod = '-';}
        timerUI.UpdateTextColour(textColor, ref textDiffObject);
        timerUI.UpdateText($"{mod}{Constants.FormatTime(Mathf.Abs(difference))}", ref textDiffObject);
        timerUI.SetActive(true, ref textDiffObject);
    }

    public void EndRace()
    {
        if (!car.IsAI)
        {
            timerUI.UpdateTextColour(Color.green, ref timerUI.time);
            timerUI.UpdateTextColour(Color.green, ref timerUI.position);
        }
    }

    public void Reset()
    {
        timerUI.SetActive(false, ref timerUI.leaderboardStatus);
        timerUI.UpdateText("Sending record data...", ref timerUI.leaderboardStatus);
        timerUI.UpdateTextColour(Color.white, ref timerUI.time);
        if (car.gameMode == Constants.GameMode.TimeAttack)
        {
            timerUI.UpdateText("00:00.00", ref timerUI.time);
            timerUI.UpdateText("Sector 1 --:--.--", ref timerUI.timeS1);
            timerUI.UpdateText("Sector 2 --:--.--", ref timerUI.timeS2);
            timerUI.UpdateText("Sector 3 --:--.--", ref timerUI.timeS3);
        }
        else
        {
            timerUI.UpdateText($"-/{PlayerPrefs.GetFloat("Bots") + 1}", ref timerUI.position);
            timerUI.UpdateText($"Current lap\n{Mathf.Clamp(car.CPosition.lap, 1, car.NumLaps)}/{car.NumLaps}", ref timerUI.currentLap);
        }
        timerUI.SetActive(false, ref timerUI.timeDiffS1);
        timerUI.SetActive(false, ref timerUI.timeDiffS2);
        timerUI.SetActive(false, ref timerUI.timeDiffS3);
    }

    public void ResetTimer(float hs)
    {
        if (hs == -1)
        {
            timerUI.UpdateText("Best time\n-:--.--", ref timerUI.bestTime);
        }
        else
        {
            timerUI.UpdateText($"Best time\n{Constants.FormatTime(hs)}", ref timerUI.bestTime);
        }
    }
}