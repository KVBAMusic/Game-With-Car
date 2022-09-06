using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarTimer : MonoBehaviour, ICarComponent
{
    CarBrain car;
    GameObject leaderboardTime, afterRace;
    HighscoreManager hm;
    int currentScene;
    float t;
    [HideInInspector] public float totalLapTime, sector1Time, sector2Time, sector3Time;
    [HideInInspector] public float practiceOffsetTime = 0f;
    Pause pause;

    TimerBehaviour timerB;

    private void Awake()
    {
        car = GetComponent<CarBrain>();
        car.CLapTracker.OnRaceStart += StartRace;
        car.OnReset += Reset;
        car.CLapTracker.OnCheckpointReached += CheckpointReached;
        car.CLapTracker.OnRaceEnd += EndRace;
        timerB = FindObjectOfType<TimerBehaviour>();
    }

    void Start()
    {
        if (!car.IsAI)
        {
            try { afterRace = GameObject.Find("after race"); Debug.Log("assigned afterRace");}
            catch (NullReferenceException) { return; }
            if (afterRace != null) afterRace.SetActive(false);
            currentScene = SceneManager.GetActiveScene().buildIndex - 1;
            pause = FindObjectOfType<Pause>();
            hm = FindObjectOfType<HighscoreManager>();
            leaderboardTime = GameObject.Find("Leaderboard");
            timerB.timer.Stop();
            ResetTimer();
        }
    }

    public void ResetTimer()
    {
        timerB.timer.Stop();
        timerB.timer.SetTime(0);
        if (!car.IsAI)
        {
            float hs = hm.GetHighscore(currentScene)[0];
            car.CUIController.ResetTimer(hs);
            afterRace.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (car.CMovement.isInPractice) car.CMovement.createCheckpoint();
        
    }
    public float GetCurrentTime() => timerB.timer.time - practiceOffsetTime;
    public IEnumerator SetSavedTimes(float time)
    {
        yield return practiceOffsetTime += t - time;
    }

    public void StartRace(object sender, EventArgs e)
    {
        if (!car.IsAI)
        {
            timerB.timer.Start();
            car.CUIController.StartRace();
        }
    }

    public void NewLap(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public void CheckpointReached(object sender, EventArgs e)
    {
        StoreSectorTime();
    }

    void StoreSectorTime()
    {
        int cp = car.CPosition.checkpoint;
        Debug.Log(cp);
        float prevSector = cp switch
        {
            1 => 0f,                            
            2 => sector1Time,                   
            3 => sector1Time + sector2Time,
            _ => float.NaN
        };

        float hsSectorTime = hm.GetHighscore(currentScene)[cp];
        float sectorTime = timerB.timer.time - prevSector;

        switch (cp)
        {
            case 1:
                sector1Time = sectorTime;
                break;
            case 2:
                sector2Time = sectorTime;
                break;
            case 3:
                sector3Time = sectorTime;
                break;
            default:
                break;
        }
        car.CUIController.DisplaySectorTime();
        if (hsSectorTime != -1)
        {
            car.CUIController.DisplaySectorDifference(sectorTime - hsSectorTime);
        }
    }

    public void EndRace(object sender, EventArgs e)
    {
        if (!car.IsAI) 
        {
            afterRace.SetActive(true);
            car.CUIController.EndRace();
            if (!car.CMovement.isInPractice && car.gameMode == Constants.GameMode.TimeAttack)
            {
                timerB.timer.Stop();
                StoreSectorTime();
                bool isHS = hm.CheckForHighscore(totalLapTime, sector1Time, sector2Time, sector3Time);
                if (isHS && PlayerPrefs.HasKey("username"))
                {
                    leaderboardTime.GetComponent<Text>().enabled = true;
                    if (Constants.isTestBuild)
                    {
                        car.CUIController.timerUI.UpdateText("You cannot send times to the leaderboard in test build.", ref car.CUIController.timerUI.leaderboardStatus);
                    }
                    else
                    {
                        leaderboardTime.GetComponent<LeaderboardSend>().Send();
                    }
                }
            }
        }
    }

    public void Reset(object sender, EventArgs e)
    {
        ResetTimer();
    }
}
