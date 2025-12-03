using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Login_Scene";


    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float blinkSpeed = 0.75f;

    private void Start()
    {
        Time.timeScale = 1f;

        if (text == null)
            text = GetComponent<TextMeshProUGUI>();

        StartCoroutine(BlinkRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex + 1);
        }
    }

    private System.Collections.IEnumerator BlinkRoutine()
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
