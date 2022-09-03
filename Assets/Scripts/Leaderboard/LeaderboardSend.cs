using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LeaderboardSend : MonoBehaviour
{
    

    public void Send()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        string text = Constants.FormatTime(FindObjectOfType<HighscoreManager>().GetHighscore(index - 1)[0]);
        string s1 = Constants.FormatTime(FindObjectOfType<HighscoreManager>().GetHighscore(index - 1)[1]);
        string s2 = Constants.FormatTime(FindObjectOfType<HighscoreManager>().GetHighscore(index - 1)[2]);
        string s3 = Constants.FormatTime(FindObjectOfType<HighscoreManager>().GetHighscore(index - 1)[3]);


        string time = LeaderboardHandle.SendTimeToServer(PlayerPrefs.GetString("username"),
                                            PlayerPrefs.GetString("password"),
                                            text, s1, s2, s3, PlayerPrefs.GetInt("Engine") + 1, index);
        string color = LeaderboardHandle.UpdateCar(PlayerPrefs.GetString("username"),
                                PlayerPrefs.GetString("password"),
                                PlayerPrefs.GetInt("Car Colour"),
                                PlayerPrefs.GetInt("Car Model"));
        StartCoroutine(SendLeaderboard(time));
        StartCoroutine(SendLeaderboard(color));
    }
    IEnumerator SendLeaderboard(string _url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_url))
        {
            print(_url);
            yield return webRequest.SendWebRequest();
            string received = webRequest.downloadHandler.text;
            print(received);
            if (received == "success" || received == "poggers")
            {
                GetComponent<Text>().color = Color.green;
                GetComponent<Text>().text = "Sent to leaderboard!";
            }
            else
            {
                GetComponent<Text>().color = Color.red;
                GetComponent<Text>().text = "Something went wrong";
            }
        }
    }
}
