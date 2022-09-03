using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    public GameObject pauseScreen, speedGauge, miniMap, pauseMenu, settingsMenu, fpsDisplay;
    public AudioController ac;
    public bool paused = false;
    Keyboard kb;
    Gamepad pad;


    private void Start()
    {
        Cursor.visible = false;
        ac.PlaySound("Car Engine");
        ac.PlaySound("Music");
        kb = InputSystem.GetDevice<Keyboard>();
        pad = InputSystem.GetDevice<Gamepad>();
    }

    void Update()
    {
        if (CheckForPauseKey(0) || CheckForPauseKey(1))
        {
            paused = !paused;
            if (!paused)
            {
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }

        }
        Time.timeScale = (paused ? 0f : 1f);
        pauseScreen.SetActive(paused);
        if (paused) ac.Pause("Car Engine");
        else ac.Unpause("Car Engine");
    }
    private bool CheckForPauseKey(uint device)
    {
        switch (device)
        {
            case 0: // keyboard
                try
                {
                    return kb.escapeKey.wasPressedThisFrame;
                }
                catch (System.NullReferenceException)
                {
                    return false;
                }
            case 1: // gamepad
                try
                {
                    return pad.startButton.wasPressedThisFrame;
                }
                catch (System.NullReferenceException)
                {
                    return false;
                }
            default:
                return false;
        }
    }

    public void SetPauseState(bool state)
    {
        paused = state;
    }
}
