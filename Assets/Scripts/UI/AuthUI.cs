using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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
                StartCoroutine(LoadGameDelayed());

                // Load game scene
                SceneManager.LoadScene("Game_Scene");
            },
            onError: err =>
            {
                statusText.text = "Login failed: " + err;
            });
    }

    private IEnumerator LoadGameDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Game_Scene");
    }
}
