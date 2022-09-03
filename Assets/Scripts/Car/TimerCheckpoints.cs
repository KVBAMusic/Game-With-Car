using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerCheckpoints : MonoBehaviour
{
    GameObject leaderboardTime, afterRace;
    AudioController ac;
    public AudioClip lap, checkpointSound;
    HighscoreManager hm;
    [HideInInspector] public int checkpoint = 0;
    int currentScene;
    float t;
    float totalLapTime, sector1Time, sector2Time, sector3Time;
    [HideInInspector] public float practiceOffsetTime = 0f;
    Pause pause;

    public bool isAI;
    float laps;
    [HideInInspector] public int currentLap = 1;
    [HideInInspector] public int position = 1;
    Constants.GameMode gameMode;

    TimerBehaviour timerB;
    TimerUIUpdate timerUI;

    private void Awake()
    {
        if (!isAI)
        {
            try { afterRace = GameObject.Find("after race"); }
            catch (NullReferenceException) { return; }
            if (afterRace != null) afterRace.SetActive(false);
        }
        gameMode = (Constants.GameMode)PlayerPrefs.GetInt("Game Mode");
        laps = PlayerPrefs.GetFloat("Laps");
        timerB = FindObjectOfType<TimerBehaviour>();
        timerUI = FindObjectOfType<TimerUIUpdate>();
    }

    void Start()
    {
        if (!isAI)
        {
            currentScene = SceneManager.GetActiveScene().buildIndex - 1;
            pause = FindObjectOfType<Pause>();
            if (gameMode == Constants.GameMode.TimeAttack)
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
                timerUI.UpdateText($"Current lap\n{Mathf.Clamp(currentLap, 1, laps)}/{laps}", ref timerUI.currentLap);
            }
            ac = FindObjectOfType<AudioController>();
            hm = FindObjectOfType<HighscoreManager>();
            leaderboardTime = GameObject.Find("Leaderboard");
            timerB.timer.Stop();
            ResetTimer();
            
        }
    }

    private void Update()
    {
        if (!isAI)
        {
            if (checkpoint != 4)
            {
                timerUI.UpdateText(Constants.FormatTime(timerB.timer.time), ref timerUI.time);
                timerUI.UpdateText($"Current lap\n {Mathf.Clamp(currentLap, 1, laps)}/{laps}", ref timerUI.currentLap);
                timerUI.UpdateText($"{position}/{PlayerPrefs.GetFloat("Bots") + 1}", ref timerUI.position);
            }
            Cursor.visible = (checkpoint == 4 || pause.paused);
        }
    }

    public void ResetTimer()
    {
        timerB.timer.Stop();
        timerB.timer.SetTime(0);
        checkpoint = 0;
        if (!isAI)
        {
            float hs = hm.GetHighscore(currentScene)[0];
            if (hs == -1)
            {
                timerUI.UpdateText("Best time\n-:--.--", ref timerUI.bestTime);
            }
            else
            {
                timerUI.UpdateText($"Best time\n{Constants.FormatTime(hs)}", ref timerUI.bestTime);
            }
            timerUI.SetActive(false, ref timerUI.leaderboardStatus);
            timerUI.UpdateText("Sending record data...", ref timerUI.leaderboardStatus);
            timerUI.UpdateTextColour(Color.white, ref timerUI.time);
            if (gameMode == Constants.GameMode.TimeAttack)
            {
                timerUI.UpdateText("00:00.00", ref timerUI.time);
                timerUI.UpdateText("Sector 1 --:--.--", ref timerUI.timeS1);
                timerUI.UpdateText("Sector 2 --:--.--", ref timerUI.timeS2);
                timerUI.UpdateText("Sector 3 --:--.--", ref timerUI.timeS3);
            }
            else
            {
                timerUI.UpdateText($"-/{PlayerPrefs.GetFloat("Bots") + 1}", ref timerUI.position);
                currentLap = 1;
                timerUI.UpdateText($"Current lap\n{Mathf.Clamp(currentLap, 1, laps)}/{laps}", ref timerUI.currentLap);
            }
            timerUI.SetActive(false, ref timerUI.timeDiffS1);
            timerUI.SetActive(false, ref timerUI.timeDiffS2);
            timerUI.SetActive(false, ref timerUI.timeDiffS3);
            afterRace.SetActive(false);
        }
        else
        {
            if (gameMode == Constants.GameMode.SingleRace)
            {
                currentLap = 1;
            }
        }
    }

    void CompareSectorTimes(Text text, int checkpoint, float sectorTime)
    {
        if (hm.GetHighscore(currentScene)[checkpoint] !=-1)
        {
            string textOut = "";
            float offset = 0;
            if (sectorTime > hm.GetHighscore(currentScene)[checkpoint])
            {
                textOut = "+";
                timerUI.UpdateTextColour(Color.red, ref text);
            }
            else if (sectorTime < hm.GetHighscore(currentScene)[checkpoint])
            {
                textOut = "-";
                timerUI.UpdateTextColour(Color.green, ref text);
                offset = .01f;
            }
            else
                timerUI.UpdateTextColour(Color.white, ref text);
            textOut += Constants.FormatTime(Mathf.Abs(hm.GetHighscore(currentScene)[checkpoint] - sectorTime) + offset);
            text.text = textOut;
            timerUI.UpdateText(textOut, ref text);
            timerUI.SetActive(true, ref text);
        }
    }

    public void OnCheckpointHit(int checkpoint, AudioClip sound, Text textObject, Text textObjectDiff, float previousSectorTime, ref float sectorTime)
    {
        this.checkpoint = checkpoint;
        if (!isAI)
        {
            sectorTime = timerB.timer.time - previousSectorTime;
            timerUI.UpdateText($"Sector {checkpoint - 1} {Constants.FormatTime(sectorTime)}", ref textObject);
            ac.SetClip("Checkpoint", sound);
            if (gameMode == Constants.GameMode.TimeAttack) 
                CompareSectorTimes(textObjectDiff, 1, sectorTime);
            ac.PlaySound("Checkpoint");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement pM = GameObject.FindGameObjectWithTag("Car").GetComponent<PlayerMovement>(); // sum practice things
        switch (other.gameObject.tag)
        {
            case "Start":
                if (checkpoint == 3 - practiceOffsetTime)
                {
                    OnCheckpointHit(4, checkpointSound, timerUI.timeS3, timerUI.timeDiffS3, sector2Time, ref sector3Time);
                    if (gameMode == Constants.GameMode.TimeAttack)
                    {
                        if (!isAI)
                        {
                            timerB.timer.Stop();
                            totalLapTime = timerB.timer.time;
                            timerUI.UpdateTextColour(Color.green, ref timerUI.time);
                            CompareSectorTimes(timerUI.timeDiffS3, 3, sector3Time);
                            afterRace.SetActive(true);
                        }
                    }
                    else
                    {
                        if (currentLap == laps)
                        {
                            checkpoint = 4;
                            if (!isAI)
                            {
                                timerUI.UpdateTextColour(Color.green, ref timerUI.position);
                                afterRace.SetActive(true);
                            }
                        }
                        else
                        {
                            checkpoint = 1;
                            currentLap += 1;
                            pM.botCheckpoint = -1;
                            if (currentLap == laps && !isAI)
                            {
                                // do somethin
                                ac.PlaySound("Final Lap");
                                timerUI.SetActiveForTime(3, ref timerUI.finalLap);
                            }
                        }
                    }
                    if (!pM.isInPractice && gameMode == Constants.GameMode.TimeAttack && !isAI)
                    {
                        bool isHS = hm.CheckForHighscore(totalLapTime, sector1Time, sector2Time, sector3Time);
                        if (isHS && PlayerPrefs.HasKey("username"))
                        {
                            leaderboardTime.GetComponent<Text>().enabled = true;
                            if (Constants.isTestBuild)
                            {
                                timerUI.UpdateText("You cannot send times to the leaderboard in test build.", ref timerUI.leaderboardStatus);
                            }
                            else
                            {
                                leaderboardTime.GetComponent<LeaderboardSend>().Send();
                            }

                        }
                    }
                }
                break;
            case "CP1":
                if (checkpoint == 1)
                {
                    OnCheckpointHit(2, checkpointSound, timerUI.timeS1, timerUI.timeDiffS1, 0, ref sector1Time);
                }
                break;
            case "CP2":
                if (checkpoint == 2)
                {
                    OnCheckpointHit(3, checkpointSound, timerUI.timeS2, timerUI.timeDiffS2, sector1Time, ref sector2Time);
                }
                break;
        }
        if (pM.isInPractice) pM.createCheckpoint();
        
    }

    public void StartLap()
    {
        checkpoint = 1;
        if (!isAI)
        {
            ac.SetClip("Checkpoint", lap);
            timerB.timer.Start();
            timerUI.UpdateTextColour(Color.yellow, ref timerUI.time);
            timerUI.UpdateTextColour(Color.yellow, ref timerUI.position);
            ac.PlaySound("Checkpoint");
        }
    }
    public float GetCurrentTime() => timerB.timer.time - practiceOffsetTime;
    public int GetCurrentChechpoint() => checkpoint;
    public IEnumerator SetSavedTimes(float time)
    {
        yield return practiceOffsetTime += t - time;
    }
}
