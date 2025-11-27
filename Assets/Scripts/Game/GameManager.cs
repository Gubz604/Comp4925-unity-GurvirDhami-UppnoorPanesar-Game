using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Gameplay")]
    public EnemyGroup enemyGroup;
    public GameObject enemyPrefab;
    public int rows = 4;
    public int columns = 8;
    public float spacingX = 0.8f;
    public float spacingY = 0.6f;
    public int startingLives = 3;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text statusText;

    private int _currentWave = 1;
    private int _score = 0;
    private int _lives;
    private int _enemiesRemaining;
    private bool _isGameOver;

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
        _lives = startingLives;
        UpdateUI();
        StartWave();
    }

    // ---------- Waves / Spawning ----------

    private void StartWave()
    {
        statusText.text = $"Wave {_currentWave}";
        ClearExistingEnemies();
        SpawnFormation();
    }

    private void ClearExistingEnemies()
    {
        var toDestroy = new System.Collections.Generic.List<GameObject>();
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
        _enemiesRemaining = rows * columns;

        // center formation around enemyGroup position
        float startX = -(columns - 1) * spacingX * 0.5f;
        float startY = 0f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 localPos = new Vector3(
                    startX + col * spacingX,
                    startY - row * spacingY,
                    0f);

                GameObject enemyObj = Instantiate(enemyPrefab, enemyGroup.transform);
                enemyObj.transform.localPosition = localPos;
            }
        }
    }

    // ---------- Events from other scripts ----------

    public void OnEnemyKilled(int scoreValue, Vector3 deathPos)
    {
        _score += scoreValue;
        _enemiesRemaining--;

        // TODO: spawn explosion particle here later

        if (_enemiesRemaining <= 0)
        {
            // wave cleared
            StartCoroutine(HandleWaveCleared());
        }

        UpdateUI();
    }

    private IEnumerator HandleWaveCleared()
    {
        statusText.text = "Wave cleared!";
        yield return new WaitForSeconds(1.5f);

        _currentWave++;
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

    private void GameOver()
    {
        _isGameOver = true;
        statusText.text = "GAME OVER";
        // TODO: restart or return to menu later
    }

    // ---------- UI ----------

    private void UpdateUI()
    {
        if (waveText) waveText.text = $"Wave: {_currentWave}";
        if (scoreText) scoreText.text = $"Score: {_score}";
        if (livesText) livesText.text = $"Lives: {_lives}";
    }
}
