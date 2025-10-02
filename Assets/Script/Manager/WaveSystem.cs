using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("StartButton & SpeedButton")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button speedButton;

    private int currentWaveIndex = -1;
    private bool isGameCleared = false;
    private bool isSeepdActive = false;   

    private const float normalTimeScale = 1f;
    private const float fastTimeScale = 2f;

    public int CurrentWave => currentWaveIndex + 1;
    public int MaxWave => waves.Length;
    public bool IsGameCleared => isGameCleared;
   
    private void Start()
    {
        if(speedButton != null)
        {
            speedButton.interactable = false;
            speedButton.onClick.AddListener(ToggleSpeed);
        }
        Time.timeScale = normalTimeScale;
    }

    public void StartWave()
    {
        if( enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length -1)
        {
            currentWaveIndex++;

            if(startButton != null)
            {
                startButton.interactable = false;
            }
            if(speedButton != null)
            {
                speedButton.interactable = true;
            }

            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
    public void ToggleSpeed()
    {
        if (speedButton != null && speedButton.interactable)
        {
            isSeepdActive = !isSeepdActive;
            Time.timeScale = isSeepdActive ? fastTimeScale : normalTimeScale;
        }
    }
    private void Update()
    {
        if (currentWaveIndex >= 0 && enemySpawner.EnemyList.Count == 0)
        {
            if (currentWaveIndex < waves.Length)
            {
                if (isSeepdActive)
                {
                    isSeepdActive = false;
                    Time.timeScale = normalTimeScale;
                }

                if (speedButton != null) speedButton.interactable = false;

                if (startButton != null && currentWaveIndex < waves.Length - 1)
                {
                    startButton.interactable = true;
                }
            }

            if (!isGameCleared && currentWaveIndex == waves.Length - 1)
            {
                isGameCleared = true;               

                if (startButton != null) startButton.interactable = false;
            }
        }
    }
}
[System.Serializable]
public struct Wave
{
    public float spawnTime;
    public int maxEnemyCount;
    public GameObject[] enemyPrefabs;
}
