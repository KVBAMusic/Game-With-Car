using UnityEngine;
using UnityEngine.UI;

public class AccountLogging : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public Text statusText;
    public CheckSavedAccount savedAccount;

    public void Register()
    {
        string hashPassword = LeaderboardHandle.hashPassword(password.text);
        bool isSuccess = LeaderboardHandle.RegisterAccount(username.text, hashPassword);
        if (isSuccess)
        {
            statusText.text = " ";
            statusText.color = Color.green;
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", hashPassword);
            savedAccount.setSettingsScreen();
        }
        else
        {
            statusText.text = "Username is already taken!";
            statusText.color = Color.red;
        }
    }
    public void Login()
    {
        string hashPassword = LeaderboardHandle.hashPassword(password.text);
        bool isSuccess = LeaderboardHandle.LoginAccount(username.text, hashPassword);
        if (isSuccess)
        {
            statusText.text = " ";
            statusText.color = Color.green;
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", hashPassword);
            savedAccount.setSettingsScreen();

        }
        else
        {
            statusText.text = "Username or password is wrong!";
            statusText.color = Color.red;
        }
    }
}
