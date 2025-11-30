using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


[System.Serializable]
public class ProgressRequest
{
    public int wave;
    public int score;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ---------- Gameplay ----------
    [Header("Gameplay")]
    public EnemyGroup enemyGroup;

    // Different enemy prefabs for different rows
    public GameObject enemySmallPrefab;
    public GameObject enemyMediumPrefab;
    public GameObject enemyLargePrefab;

    public int rows = 5;
    public int columns = 11;
    public float spacingY = 0.55f;
    public int startingLives = 3;

    // ---------- UI ----------
    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text statusText;
    public TMP_Text bestText;

    // ---------- Game Over ----------
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    public bool IsGameOver => _isGameOver;


    // ---------- State ----------
    private int _currentWave = 1;
    private int _score = 0;
    private int _lives;
    private int _enemiesRemaining;
    private bool _isGameOver;
    private bool _isWaveClearing;

    // Best progress from server
    private int _bestScore = 0;
    private int _bestWave = 1;

    private Vector3 _enemyGroupStartPos;

    // -------------------- Unity Lifecycle --------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (enemyGroup != null)
        {
            _enemyGroupStartPos = enemyGroup.transform.position;
        }

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(false);
        }

        _currentWave = 1;
        _score = 0;
        _lives = startingLives;
        _isGameOver = false;
        _isWaveClearing = false;

        UpdateUI();
        StartCoroutine(LoadProgressAndStartWave());
    }

    private void Update()
    {
        // Allow forcing game over with Q (only if game isn't already over)
        if (!_isGameOver && Input.GetKeyDown(KeyCode.Q))
        {
            GameOver();
        }
    }


    // -------------------- Progress (Server) --------------------

    private IEnumerator LoadProgressAndStartWave()
    {
        yield return LoadProgressFromServer();
        StartWave();
    }

    private IEnumerator LoadProgressFromServer()
    {
        using (var request = UnityWebRequest.Get(ApiConfig.BASE_URL + "/api/progress"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Failed to load progress: " + request.error);
                yield break;
            }

            var json = request.downloadHandler.text;
            Debug.Log("Progress GET: " + json);

            ProgressResponse data = null;
            try
            {
                data = JsonUtility.FromJson<ProgressResponse>(json);
            }
            catch
            {
                Debug.LogWarning("Could not parse progress JSON");
            }

            if (data != null)
            {
                _bestScore = Mathf.Max(0, data.bestScore);
                _bestWave = Mathf.Max(1, data.bestWave);

                _currentWave = 1;
                _score = 0;

                if (statusText)
                {
                    statusText.text = $"Best: {_bestScore} (Wave {_bestWave})";
                }

                UpdateUI();
            }
        }

        yield return new WaitForSeconds(1f);
        if (statusText)
        {
            statusText.text = "";
        }
    }

    private void SaveProgress()
    {
        StartCoroutine(SaveProgressCoroutine());
    }

    private IEnumerator SaveProgressCoroutine()
    {
        var payload = new ProgressRequest
        {
            wave = _currentWave,
            score = _score
        };

        string json = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (var request = new UnityWebRequest(ApiConfig.BASE_URL + "/api/progress", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Failed to save progress: " + request.error +
                                 " :: " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Progress saved OK");
            }
        }
    }

    // -------------------- Wave / Spawning --------------------

    private IEnumerator ClearStatusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (statusText)
            statusText.text = "";
    }

    private void StartWave()
    {
        Debug.Log($"[GameManager] StartWave() called. Wave = {_currentWave}");

        _isWaveClearing = false;

        if (enemyGroup == null)
        {
            Debug.LogError("[GameManager] enemyGroup is NULL! Assign it in the Inspector.");
            return;
        }

        enemyGroup.transform.position = _enemyGroupStartPos;

        if (statusText)
        {
            statusText.text = $"Wave {_currentWave}";
            StartCoroutine(ClearStatusAfterDelay(3f));
        }

        ClearExistingEnemies();
        SpawnFormation();
    }

    private void ClearExistingEnemies()
    {
        var toDestroy = new List<GameObject>();
        foreach (Transform child in enemyGroup.transform)
        {
            toDestroy.Add(child.gameObject);
        }
        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    private void SpawnFormation()
    {
        Debug.Log($"[GameManager] SpawnFormation() rows={rows}, columns={columns}");

        _enemiesRemaining = 0;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[GameManager] Camera.main is NULL. Cannot spawn enemies.");
            return;
        }

        float leftViewportX = 0.1f;
        float rightViewportX = 0.9f;

        Vector3 leftWorld = cam.ViewportToWorldPoint(new Vector3(leftViewportX, 0.8f, cam.nearClipPlane));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(rightViewportX, 0.8f, cam.nearClipPlane));

        float totalWidth = rightWorld.x - leftWorld.x;
        float spacingXLocal = totalWidth / (columns - 1);
        spacingXLocal *= 0.9f;

        float spacingYLocal = spacingY;

        float startX = -(columns - 1) * spacingXLocal * 0.5f;
        float startY = 0f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject prefabToUse = GetEnemyPrefabForRow(row);
                if (prefabToUse == null)
                {
                    Debug.LogError($"[GameManager] prefabToUse is NULL for row={row}.");
                    continue;
                }

                Vector3 localPos = new Vector3(
                    startX + col * spacingXLocal,
                    startY - row * spacingYLocal,
                    0f);

                GameObject enemyObj = Instantiate(prefabToUse, enemyGroup.transform);
                enemyObj.transform.localPosition = localPos;

                _enemiesRemaining++;

                Enemy enemyComp = enemyObj.GetComponent<Enemy>();
                if (enemyComp != null)
                {
                    enemyComp.row = row;
                    enemyComp.col = col;
                }
                else
                {
                    Debug.LogWarning($"[GameManager] Spawned enemy '{prefabToUse.name}' has no Enemy component.");
                }
            }
        }

        Debug.Log($"[GameManager] Formation completed. _enemiesRemaining = {_enemiesRemaining}");
    }

    private GameObject GetEnemyPrefabForRow(int row)
    {
        if (row == 0)
            return enemyLargePrefab;
        if (row == 1 || row == 2)
            return enemyMediumPrefab;
        return enemySmallPrefab;
    }

    // -------------------- Events from other scripts --------------------

    public void OnEnemyKilled(int scoreValue, Vector3 deathPos)
    {
        if (_isGameOver) return;

        _score += scoreValue;
        _enemiesRemaining = Mathf.Max(0, _enemiesRemaining - 1);

        Debug.Log($"[GameManager] Enemy killed. Remaining = {_enemiesRemaining}");

        if (_enemiesRemaining == 0 && !_isWaveClearing)
        {
            _isWaveClearing = true;
            StartCoroutine(HandleWaveCleared());
        }

        UpdateUI();
    }

    private IEnumerator HandleWaveCleared()
    {
        SaveProgress();

        if (statusText)
            statusText.text = "Wave cleared!";

        yield return new WaitForSeconds(1.5f);

        _currentWave++;
        if (statusText)
            statusText.text = "";

        StartWave();
    }

    public void OnPlayerHit(int damage)
    {
        if (_isGameOver) return;

        _lives -= damage;
        if (_lives <= 0)
        {
            _lives = 0;
            GameOver();
        }
        UpdateUI();
    }

    public void OnEnemyReachedBottom()
    {
        if (_isGameOver) return;
        GameOver();
    }

    public void GameOver()
    {
        _isGameOver = true;

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }
        if (gameOverScoreText)
        {
            gameOverScoreText.text = $"Score: {_score}";
        }

        Time.timeScale = 0f;
        SaveProgress();
    }

    public void OnPlayAgainButton()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Reload current scene
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }


    // -------------------- UI --------------------

    private void UpdateUI()
    {
        if (waveText) waveText.text = $"Wave: {_currentWave}";
        if (scoreText) scoreText.text = $"Score: {_score}";
        if (livesText) livesText.text = $"Lives: {_lives}";
        if (bestText) bestText.text = $"Best: {_bestScore} (Wave {_bestWave})";
    }
}
