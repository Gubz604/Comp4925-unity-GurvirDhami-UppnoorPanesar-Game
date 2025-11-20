using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class AuthRequest
{
    public string username;
    public string password;
}

[System.Serializable]
public class AuthResponse
{
    public string username;
    // add more fields later if needed
}

public class AuthService : MonoBehaviour
{
    public static AuthService Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("AuthService is alive! BASE_URL = " + ApiConfig.BASE_URL);
    }


    // -------- Register --------
    public void Register(string username, string password,
                         System.Action<AuthResponse> onSuccess,
                         System.Action<string> onError)
    {
        StartCoroutine(RegisterRoutine(username, password, onSuccess, onError));
    }

    private IEnumerator RegisterRoutine(string username, string password,
                                        System.Action<AuthResponse> onSuccess,
                                        System.Action<string> onError)
    {
        var payload = new AuthRequest { username = username, password = password };
        string json = JsonUtility.ToJson(payload);

        using (var request = new UnityWebRequest(ApiConfig.BASE_URL + "/api/auth/register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error + " :: " + request.downloadHandler.text);
            }
            else
            {
                var respJson = request.downloadHandler.text;
                var resp = JsonUtility.FromJson<AuthResponse>(respJson);
                onSuccess?.Invoke(resp);
            }
        }
    }

    // -------- Login --------
    public void Login(string username, string password,
                      System.Action<AuthResponse> onSuccess,
                      System.Action<string> onError)
    {
        StartCoroutine(LoginRoutine(username, password, onSuccess, onError));
    }

    private IEnumerator LoginRoutine(string username, string password,
                                     System.Action<AuthResponse> onSuccess,
                                     System.Action<string> onError)
    {
        var payload = new AuthRequest { username = username, password = password };
        string json = JsonUtility.ToJson(payload);

        using (var request = new UnityWebRequest(ApiConfig.BASE_URL + "/api/auth/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error + " :: " + request.downloadHandler.text);
            }
            else
            {
                var respJson = request.downloadHandler.text;
                var resp = JsonUtility.FromJson<AuthResponse>(respJson);
                onSuccess?.Invoke(resp);
            }
        }
    }
}
