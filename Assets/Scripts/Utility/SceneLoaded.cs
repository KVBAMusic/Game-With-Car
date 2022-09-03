using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaded : MonoBehaviour
{
    public Transform[] initCarTransforms;
    Vector3 initPos, initEuler;
    public AudioClip lap, check;
    AudioController ac;
    public Sprite[] countdownSprites = new Sprite[4];
    public Image image;

    public Constants.GameMode gameMode;

    private void Awake()
    {
        float botCount = PlayerPrefs.GetFloat("Bots");
        // playerStartPosition = Random.Range(0, botCount);
        CarLoader cl = GetComponent<CarLoader>();
        ac = FindObjectOfType<AudioController>();
        GameObject car;

        gameMode = (Constants.GameMode)PlayerPrefs.GetInt("Game Mode");

        if (gameMode == Constants.GameMode.SingleRace)
        {
            for (int i = 0; i <= botCount; i++)
            {
                initPos = initCarTransforms[i].position;
                initEuler = initCarTransforms[i].eulerAngles;
                if (i == botCount) // This way the player always starts last
                {
                    if (PlayerPrefs.HasKey("Car Model") && PlayerPrefs.HasKey("Engine"))
                        car = cl.GetCarPrefab(PlayerPrefs.GetInt("Car Model"), PlayerPrefs.GetInt("Engine"), false, false);
                    else car = cl.GetCarPrefab(0, 0, false, false);
                    car.GetComponent<TimerCheckpoints>().lap = lap;
                    car.GetComponent<TimerCheckpoints>().checkpointSound = check;
                }
                else
                {
                    car = cl.GetCarPrefab(Random.Range(0, 7), Random.Range(0, 3), false, true);
                }
                Instantiate(car, initPos, Quaternion.Euler(initEuler));
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("Car Model") && PlayerPrefs.HasKey("Engine"))
                car = cl.GetCarPrefab(PlayerPrefs.GetInt("Car Model"), PlayerPrefs.GetInt("Engine"), false, false);
            else car = cl.GetCarPrefab(0, 0, false, false);
            initPos = initCarTransforms[0].position;
            initEuler = initCarTransforms[0].eulerAngles;
            Instantiate(car, initPos, Quaternion.Euler(initEuler));
        }


        StartCoroutine(DelayedCountDown());
    }

    /// <summary>
    /// Starts countdown on the start of the race
    /// </summary>
    IEnumerator DelayedCountDown()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(nameof(CountDown));
    }

    /// <summary>
    /// Starts countdown on car/race reset
    /// </summary>
    public IEnumerator ResetCountDown()
    {
        StopCoroutine(nameof(CountDown));
        StartCoroutine(nameof(CountDown));
        yield return null;
    }

    /// <summary>
    /// A pre-race countdown
    /// </summary>
    public IEnumerator CountDown()
    {
        image.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            image.sprite = countdownSprites[i];
            if (i == 3)
            {
                ac.PlaySound("Countdown Go");
                GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
                foreach (var car in cars)
                {
                    car.GetComponent<PlayerMovement>().controlable = true;
                    car.GetComponent<Rigidbody>().isKinematic = false;
                    car.GetComponent<TimerCheckpoints>().StartLap();
                }


            }
            else ac.PlaySound("Countdown Hold");
            yield return new WaitForSeconds(.8f);
        }
        image.gameObject.SetActive(false);
    }
}
