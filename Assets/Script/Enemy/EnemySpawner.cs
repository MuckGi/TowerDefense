using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyHpSliderPrefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private PlayerHp playerHp;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private GameObject arrowPrefab;

    
    private Wave currentWave;
    private List<Enemy> enemyList;
    private int currentEnemyCount;
    private bool ShowArrow = false;
    public List<Enemy> EnemyList => enemyList;
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Start()
    {
        if (!ShowArrow && wayPoints.Length >= 2)
        {
            ShowArrow = true;
            ShowWaveArrow();

        }
    }
    private void ShowWaveArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab);  
        Arrow arrowMove = arrow.GetComponent<Arrow>();
        arrowMove.startPoint = wayPoints[0];
        arrowMove.endPoint = wayPoints[1];
        
    }
    private void Awake()
    {
        enemyList = new List<Enemy> ();        
    }
    public void StartWave(Wave wave)
    {
        currentWave = wave;
        currentEnemyCount = currentWave.maxEnemyCount;
        StartCoroutine("SpawnEnemy");
    }
    private IEnumerator SpawnEnemy()
    {
        int spawnEnemyCount = 0;

        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);

            SpawnEnemyHpSlider(clone);

            spawnEnemyCount++;
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }
    public void DestroyEnemy(EnemyDestroyType type,Enemy enemy, int gold)
    {
        if(type == EnemyDestroyType.Arrive)
        {
            playerHp.TakeDamage(1);
        }
        else if(type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
        }
        currentEnemyCount--;
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);
        sliderClone.transform.SetParent(canvasTransform);
        sliderClone.transform.localScale = Vector3.one;
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHpViewer>().Setup(enemy.GetComponent<EnemyHp>());
    }
}
