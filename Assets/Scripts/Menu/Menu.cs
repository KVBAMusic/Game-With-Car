using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public AudioController ac;
    public Text loggedAs, carName, carDesc;
    [SerializeField] Slider laps, bots;
    [SerializeField] Text laps_d, bots_d;

    private void Start()
    {
        laps.value = PlayerPrefs.HasKey("Laps") ? PlayerPrefs.GetFloat("Laps") : 3;
        bots.value = PlayerPrefs.HasKey("Bots") ? PlayerPrefs.GetFloat("Bots") : 7;
        laps_d.text = laps.value.ToString();
        bots_d.text = bots.value.ToString();

        ac.PlaySound("Music");

        Time.timeScale = 1;

        if (!PlayerPrefs.HasKey("Bots")) PlayerPrefs.SetFloat("Bots", 7);
        if (!PlayerPrefs.HasKey("Laps")) PlayerPrefs.SetFloat("Laps", 3);

        laps.onValueChanged.AddListener(delegate { SetLaps(); });
        bots.onValueChanged.AddListener(delegate { SetBots(); });
    }


    private void Update()
    {
        float musicVolume = PlayerPrefs.GetFloat("Music");
        if (PlayerPrefs.HasKey("username")) loggedAs.text = "Logged as: " + PlayerPrefs.GetString("username");
        else loggedAs.text = "Not logged in";
        ac.SetVolume("Music", musicVolume);
    }

    public void ChangeCarDescription(PlayerMovement car)
    {
        carName.text = car.carName;
        carDesc.text = car.carDescription;
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Set the game mode
    /// </summary>
    public void SetGameMode(int mode)
    {
        PlayerPrefs.SetInt("Game Mode", mode);
    }
    /// <summary>
    /// Set the game mode
    /// </summary>
    public void SetGameMode(Constants.GameMode mode)
    {
        PlayerPrefs.SetInt("Game Mode", (int)mode);
    }
    /// <summary>
    /// Update the number of laps
    /// </summary>
    public void SetLaps()
    {
        PlayerPrefs.SetFloat("Laps", laps.value);
        laps_d.text = laps.value.ToString();
    }

    /// <summary>
    /// Update the number of bots
    /// </summary>
    public void SetBots()
    {
        PlayerPrefs.SetFloat("Bots", bots.value);
        bots_d.text = bots.value.ToString();
    }
}