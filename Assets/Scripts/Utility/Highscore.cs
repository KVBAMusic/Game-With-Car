[System.Serializable]
public class Highscore
{
    public int trackNumber;

    public float bestTime;

    public float bestS1;
    public float bestS2;
    public float bestS3;

    public Highscore(int track, float time, float s1, float s2, float s3)
    {
        bestTime = time != null ? time : -1;
        bestS1 = s1 != null ? s1 : -1;
        bestS2 = s2 != null ? s2 : -1;
        bestS3 = s3 != null ? s3 : -1;
        trackNumber = track;
    }
}
