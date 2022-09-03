using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/" + Constants.physicsVersion + "_local_highscores.bin";
    const int tracks = Constants.numberOfTracks;
    public static void SaveTimes(HighscoreManager hm)
    {
        BinaryFormatter formatter = new();

        FileStream stream = new(path, FileMode.Create);

        Highscore[] data = hm.times;
        formatter.Serialize(stream, data);
        stream.Close();
    }
    unsafe public static Highscore[] LoadTimes()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();

            FileStream stream = new(path, FileMode.Open);
            Highscore[] data = formatter.Deserialize(stream) as Highscore[];
            stream.Close();
            if (data.Length < tracks)
            {
                Highscore[] temp = new Highscore[tracks];
                for (int i = 0; i < data.Length; i++) temp[i] = data[i];
                for (int i = temp.Length - 1; i < tracks; i++)
                {
                    temp[i] = new Highscore(i + 1, -1, -1, -1, -1);
                }

                Debug.LogWarning(temp.Length);
                Debug.LogWarning(data.Length);
                data = new Highscore[tracks];
                for (int i = 0; i < tracks; i++)
                {
                    data[i] = temp[i];
                }
                Debug.LogWarning(data.Length);
            }
            return data;

        }
        else
        {
            Debug.LogWarning("Save file not found. Creating new save file...");
            Highscore[] data = new Highscore[tracks];
            for (int i = 0; i < tracks; i++)
            {
                data[i] = new Highscore(i + 1, -1, -1, -1, -1);
            }
            return data;
        }
    }
}