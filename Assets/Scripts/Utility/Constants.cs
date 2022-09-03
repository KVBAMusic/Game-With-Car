using UnityEngine;

/// <summary>
/// <c>Constants</c> is a class containing all important constants in GWC
/// </summary>
public static class Constants
{
    public const int numberOfTracks = 15;
    public const string version = "0.4.0";
    public const string physicsVersion = "0.4.0";
    public const bool isTestBuild = true;

    /// <summary>
    /// Available game modes
    /// </summary>
    public enum GameMode { TimeAttack, SingleRace };

    /// <summary>
    /// Gets time in seconds and converts it into a formatted string
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns>A string time in mm:ss.cs format</returns>
    public static string FormatTime(float time)
    {
        float ms = Mathf.Floor(time * 100);
        float seconds = Mathf.Floor(ms / 100);
        float minutes = Mathf.Floor(seconds / 60);
        string ms_s = (ms - seconds * 100).ToString();
        string seconds_s = (seconds - minutes * 60).ToString();
        string minutes_s = minutes.ToString();
        for (int i = ms_s.Length; i < 2; i++)
        {
            ms_s = "0" + ms_s;
        }
        for (int i = seconds_s.Length; i < 2; i++)
        {
            seconds_s = "0" + seconds_s;
        }
        for (int i = minutes_s.Length; i < 2; i++)
        {
            minutes_s = "0" + minutes_s;
        }
        return minutes_s + ":" + seconds_s + "." + ms_s;
    }


}
