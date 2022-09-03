using UnityEngine;
using UnityEngine.UI;

public class ChangePassword : MonoBehaviour
{
    public InputField oldPassword, newPassword;
    public Text statusText;
    public AccountSettings accSettings;
    public void SubmitChange()
    {
        if (oldPassword.text != null || newPassword.text != null)
        {
            string _password = LeaderboardHandle.sha256_hash(oldPassword.text);
            string _newPassword = LeaderboardHandle.sha256_hash(newPassword.text);
            string reqResult = LeaderboardHandle.UpdatePassword(PlayerPrefs.GetString("username"), _password, _newPassword);
            print(reqResult);
            switch (reqResult)
            {
                case "success":
                    statusText.color = Color.green;
                    statusText.text = "Successfully changed password!";
                    PlayerPrefs.SetString("password", _newPassword);
                    oldPassword.text = null;
                    newPassword.text = null;
                    accSettings.ChangePasswordButton();
                    break;
                case "wrong":
                    statusText.color = Color.red;
                    statusText.text = "Wrong old password!";
                    break;
                case "Error":
                    statusText.color = Color.red;
                    statusText.text = "Something went wrong...";
                    break;
            }
        }
    }
}
