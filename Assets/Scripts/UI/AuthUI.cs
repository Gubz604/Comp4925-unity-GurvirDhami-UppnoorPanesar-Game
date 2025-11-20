using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class AuthUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    public void OnRegisterClicked()
    {
        statusText.text = "Registering...";
        AuthService.Instance.Register(
            usernameInput.text,
            passwordInput.text,
            onSuccess: resp =>
            {
                statusText.text = "Registered as " + resp.username;
            },
            onError: err =>
            {
                statusText.text = "Register failed: " + err;
            });
    }

    public void OnLoginClicked()
    {
        statusText.text = "Logging in...";
        AuthService.Instance.Login(
            usernameInput.text,
            passwordInput.text,
            onSuccess: resp =>
            {
                statusText.text = "Logged in as " + resp.username;
                // TODO: load your main game scene here
                Debug.Log("Login successful, proceed to main game scene.");
            },
            onError: err =>
            {
                statusText.text = "Login failed: " + err;
            });
    }
}
