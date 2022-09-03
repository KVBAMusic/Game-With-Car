using System;
using UnityEngine;
using UnityEngine.UI;
public class FPSLimiter : MonoBehaviour
{
    int targetFPS;
    public InputField input;

    private void Awake()
    {
        targetFPS = PlayerPrefs.HasKey("Max FPS") ? PlayerPrefs.GetInt("Max FPS") : 60;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
    void Update()
    {
        if (Application.targetFrameRate != targetFPS)
        {
            Application.targetFrameRate = targetFPS;
        }
    }
    public void UpdateMaxFPS()
    {
        int fps;
        try
        {
            fps = int.Parse(input.text);
        }
        catch (FormatException)
        {
            fps = 60;
        }
        Debug.LogWarning("Update: " + fps);
        PlayerPrefs.SetInt("Max FPS", fps);
        targetFPS = fps;
    }

}