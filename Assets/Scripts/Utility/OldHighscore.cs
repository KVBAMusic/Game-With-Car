[System.Serializable]
public class OldHighscore
{
    public int trackNumber;

    public float bestTime;

    public OldHighscore(float time, int track)
    {
        bestTime = time;
        trackNumber = track;
    }
}
