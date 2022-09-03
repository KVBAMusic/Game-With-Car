using UnityEngine;
using UnityEngine.UI;

public class BestTime : MonoBehaviour
{
    public SceneLoader sl;
    public HighscoreManager hm;
    public Text[] texts;

    public void ShowBestTime()
    {
        float[] highscore = hm.GetHighscore(sl.scene - 1);
        string output;
        for (int i = 0; i < 4; i++)
        {
            output = "";
            switch (highscore[i])
            {
                case -1:
                    output = "--:--.--";
                    break;
                default:
                    output = Constants.FormatTime(hm.GetHighscore(sl.scene - 1)[i]);
                    break;
            }

            texts[i].text = output;
        }

    }

    private void OnEnable()
    {
        ShowBestTime();
    }
}
