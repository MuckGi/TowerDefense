using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTMpViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHp;
    [SerializeField] private TextMeshProUGUI textPlayerGold;
    [SerializeField] private TextMeshProUGUI textWave;
    [SerializeField] private TextMeshProUGUI textEnemyCount;
    [SerializeField] private PlayerHp playerHp;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private WaveSystem waveSystem;
    [SerializeField] private EnemySpawner enemySpawner;

    private void Update()
    {
        textPlayerHp.text = playerHp.CurrentHp + "/" + playerHp.MaxHp;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }
}
