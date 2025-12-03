using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [Header("Movement")]
    public float stepInterval = 1.0f;   
    public float horizontalStep = 0.3f; 
    public float downStep = 0.15f;      
    public float viewportMargin = 0.005f;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public float minShootInterval = 2f; 
    public float maxShootInterval = 4f;

    private float _moveTimer = 0f;
    private int _direction = 1; 

    private float _shootTimer = 0f;
    private float _nextShootTime = 0f;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        ResetShootTimer();
    }

    private void Update()
    {
        if (transform.childCount == 0) return;

        // --- Movement step timer ---
        _moveTimer += Time.deltaTime;
        if (_moveTimer >= stepInterval)
        {
            _moveTimer = 0f;
            PerformStep();
        }

        // --- Shooting timer ---
        _shootTimer += Time.deltaTime;
        if (_shootTimer >= _nextShootTime)
        {
            TryShoot();
            ResetShootTimer();
        }
    }

    private void PerformStep()
    {
        // 1. Move horizontally
        transform.position += Vector3.right * (_direction * horizontalStep);

        // 2. Check for edge
        bool hitEdge = false;

        foreach (Transform child in transform)
        {
            Vector3 viewportPos = _cam.WorldToViewportPoint(child.position);

            if (viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin)
            {
                hitEdge = true;
                break;
            }
        }

        if (hitEdge)
        {
            transform.position += Vector3.down * downStep;
            _direction *= -1;
        }

        ToggleAllEnemyFrames();
    }

    private void ToggleAllEnemyFrames()
    {
        foreach (Transform child in transform)
        {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ToggleFrame();
            }
        }
    }

    private void ResetShootTimer()
    {
        _shootTimer = 0f;
        _nextShootTime = Random.Range(minShootInterval, maxShootInterval);
    }

    private void TryShoot()
    {
        if (enemyBulletPrefab == null) return;
        if (EnemyBullet.ActiveCount > 0) return; 
        if (transform.childCount == 0) return;

        List<Transform> eligibleShooters = GetEligibleShooters();
        if (eligibleShooters.Count == 0) return;

        Transform shooter = eligibleShooters[Random.Range(0, eligibleShooters.Count)];
        Vector3 spawnPos = shooter.position + Vector3.down * 0.3f;

        Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
    }

    private List<Transform> GetEligibleShooters()
    {
        Dictionary<int, Transform> bottomByColumn = new Dictionary<int, Transform>();

        foreach (Transform child in transform)
        {
            Enemy enemyComp = child.GetComponent<Enemy>();
            if (enemyComp == null) continue;

            int col = enemyComp.col;

            if (!bottomByColumn.TryGetValue(col, out Transform currentBottom))
            {
                bottomByColumn[col] = child;
            }
            else
            {
                Enemy currentEnemy = currentBottom.GetComponent<Enemy>();
                if (enemyComp.row > currentEnemy.row)   
                {
                    bottomByColumn[col] = child;
                }
            }
        }

        return new List<Transform>(bottomByColumn.Values);
    }
}
