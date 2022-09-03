using System;
using UnityEngine;
using UnityEngine.UI;

public class AccountSettings : MonoBehaviour
{
    public Text nicknameHolder;
    public CheckSavedAccount savedAccount;
    public Text statusHolder;
    public GameObject changePasswordMenu;
    private HighscoreManager hm;

    [System.Serializable]
    public class LevelTime
    {
        public int level;
        public string time;
        public string sec1;
        public string sec2;
        public string sec3;
    }
    void Start()
    {
        nicknameHolder.text = $"Logged as: {PlayerPrefs.GetString("username")}";
        hm = GameObject.Find("EventSystem").GetComponent<HighscoreManager>();
    }
    private void OnEnable()
    {
        nicknameHolder.text = $"Logged as: {PlayerPrefs.GetString("username")}";
        statusHolder.text = "";
    }
    public void Logout()
    {
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("password");
        savedAccount.setLoginScreen();
        statusHolder.text = "";
    }
    public void ChangePasswordButton()
    {
        changePasswordMenu.SetActive(!changePasswordMenu.activeSelf);
    }

    public void Save()
    {
        if (Constants.isTestBuild)
        {
            statusHolder.text = "You cannot save highscores in test build.";
            statusHolder.color = Color.red;
        }
        else
        {
            string jsonData = "{";
            for (int i = 1; i <= Constants.numberOfTracks; i++)
            {
                if (hm.GetHighscore(i - 1)[0] != -1f)
                {
                    string time = Constants.FormatTime(hm.GetHighscore(i - 1)[0]);
                    string sec1 = Constants.FormatTime(hm.GetHighscore(i - 1)[1]);
                    string sec2 = Constants.FormatTime(hm.GetHighscore(i - 1)[2]);
                    string sec3 = Constants.FormatTime(hm.GetHighscore(i - 1)[3]);
                    jsonData += $"\"{i}\":" + "{" + $"\"time\":\"{time}\",\"sec1\":\"{sec1}\",\"sec2\":\"{sec2}\",\"sec3\":\"{sec3}\",\"engine\":\"{PlayerPrefs.GetInt("Engine")+1}\"" + "},";
                }
            }
            jsonData = jsonData.Remove(jsonData.Length - 1);
            jsonData += "}";
            Debug.Log(jsonData);
            string sendResult = LeaderboardHandle.SendDataToServer(PlayerPrefs.GetString("username"), PlayerPrefs.GetString("password"), jsonData, PlayerPrefs.GetInt("Car Colour"), PlayerPrefs.GetInt("Car Model"));
            if (sendResult.Length == 0 || sendResult.Contains("wrong") || sendResult.Contains("not poggers") || sendResult.Contains("error") || sendResult.Contains("tags")
                || sendResult.Contains("go away") || sendResult.Contains("Error"))
            {
                statusHolder.text = "Something went wrong...";
                statusHolder.color = Color.red;
            }
            else
            {
                statusHolder.text = "Successfully synced data!";
                statusHolder.color = Color.green;
            }
        }
    }

    public void Sync()
    {
        if (Constants.isTestBuild)
        {
            statusHolder.text = "You cannot save highscores in test build.";
            statusHolder.color = Color.red;
        }
        else
        {
            string jsonTimes = LeaderboardHandle.GetTimesFromServer(PlayerPrefs.GetString("username"), PlayerPrefs.GetString("password"));
            Debug.Log(jsonTimes);
            if (jsonTimes == "nodata")
            {
                statusHolder.text = "No data on server";
                statusHolder.color = Color.red;
            }
            else if (jsonTimes == "Wrong username or password")
            {
                statusHolder.text = "Something went wrong... Try to relog into your account";
                statusHolder.color = Color.red;
            }
            else
            {
                LevelTime[] levels = JsonHelper.getJsonArray<LevelTime>(jsonTimes);
                foreach (LevelTime lTime in levels)
                {
                    Debug.Log(lTime.time);
                    float time2 = (float)TimeSpan.Parse("00:" + lTime.time).TotalSeconds;
                    float sec1 = (float)TimeSpan.Parse("00:" + lTime.sec1).TotalSeconds;
                    float sec2 = (float)TimeSpan.Parse("00:" + lTime.sec2).TotalSeconds;
                    float sec3 = (float)TimeSpan.Parse("00:" + lTime.sec3).TotalSeconds;
                    hm.SetHighscore(lTime.level - 1, time2, sec1, sec2, sec3);
                }
                statusHolder.text = "Successfully synced data!";
                statusHolder.color = Color.green;
            }
        }

    }
}
