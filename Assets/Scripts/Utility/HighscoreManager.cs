using UnityEngine;
using UnityEngine.SceneManagement;

public class HighscoreManager : MonoBehaviour
{
    const int levels = Constants.numberOfTracks;
    public Highscore[] times = new Highscore[levels];
    // Start is called before the first frame update
    void Start()
    {
        times = SaveSystem.LoadTimes();
    }

    public void UpdateHighscores()
    {
        SaveSystem.SaveTimes(this);
    }

    public bool CheckForHighscore(float time, float s1, float s2, float s3)
    {
        if (!Constants.isTestBuild)
        {
            int index = SceneManager.GetActiveScene().buildIndex - 1;
            if (times[index].bestTime != -1f)
            {
                if (time < times[index].bestTime)
                {
                    times[index].bestTime = time;
                    times[index].bestS1 = s1;
                    times[index].bestS2 = s2;
                    times[index].bestS3 = s3;
                    return true;
                }
                else return false;
            }
            else
            {
                times[index].bestTime = time;
                times[index].bestS1 = s1;
                times[index].bestS2 = s2;
                times[index].bestS3 = s3;
                return true;
            }
        }
        else return false;
    }

    public float[] GetHighscore(int index)
    {
        return new float[4] {
            times[index].bestTime,
            times[index].bestS1,
            times[index].bestS2,
            times[index].bestS3 };
    }
    public void SetHighscore(int index, float time, float s1, float s2, float s3)
    {
        times[index].bestTime = time;
        times[index].bestS1 = s1;
        times[index].bestS2 = s2;
        times[index].bestS3 = s3;
    }
    public void ResetTimes()
    {
        for (int i = 0; i < levels; i++)
        {
            times[i].bestTime = -1;
            times[i].bestS1 = -1;
            times[i].bestS2 = -1;
            times[i].bestS3 = -1;
        }
    }

    public void ResetThisTime()
    {

        times[SceneManager.GetActiveScene().buildIndex - 1].bestTime = -1f;
        times[SceneManager.GetActiveScene().buildIndex - 1].bestS1 = -1f;
        times[SceneManager.GetActiveScene().buildIndex - 1].bestS2 = -1f;
        times[SceneManager.GetActiveScene().buildIndex - 1].bestS3 = -1f;
    }

    private void OnDestroy()
    {
        UpdateHighscores();
    }
}
