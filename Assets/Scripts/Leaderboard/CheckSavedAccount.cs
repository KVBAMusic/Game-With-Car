using UnityEngine;

public class CheckSavedAccount : MonoBehaviour
{
    public GameObject login, profileSettings;
    void Start()
    {
        login.SetActive(false);
        profileSettings.SetActive(false);
        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
        {
            setSettingsScreen();
        }
        else
        {
            setLoginScreen();
        }
    }
    public void setLoginScreen()
    {
        login.SetActive(true);
        profileSettings.SetActive(false);
    }
    public void setSettingsScreen()
    {
        login.SetActive(false);
        profileSettings.SetActive(true);
    }

}
