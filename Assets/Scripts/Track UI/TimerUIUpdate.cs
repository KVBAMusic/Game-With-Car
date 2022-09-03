using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUIUpdate : MonoBehaviour
{
    [Header("Time Attack")]
    public GameObject timeAttackUI;
    public Text time;
    public Text timeS1;
    public Text timeS2;
    public Text timeS3;
    public Text timeDiffS1;
    public Text timeDiffS2;
    public Text timeDiffS3;
    public Text bestTime;
    public Text leaderboardStatus;

    [Header("Single Race")]
    public GameObject raceUI;
    public Text position;
    public Text currentLap;
    public Text finalLap;

    float sec;

    public void UpdateText(string text, ref Text textObject)
    {
        textObject.text = text; 
    }

    public void UpdateTextColour(Color color, ref Text textObject)
    {
        textObject.color = color;
    }

    public void SetActive(bool active, ref Text _object)
    {
        _object.gameObject.SetActive(active);
    }
    public void SetActive(bool active, ref Image _object)
    {
        _object.gameObject.SetActive(active);
    }
    public void SetActive(bool active, ref Button _object)
    {
        _object.gameObject.SetActive(active);
    }
    public void SetActive(bool active, ref GameObject _object)
    {
        _object.SetActive(active);
    }
    public void SetActiveForTime(float seconds, ref Text _object)
    {
        sec = seconds;
        StartCoroutine(ShowForTime(_object));
    }

    IEnumerator ShowForTime(Text text)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(sec);
        text.gameObject.SetActive(false);
    }
    IEnumerator ShowForTime(GameObject gameObject)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);
    }
}
