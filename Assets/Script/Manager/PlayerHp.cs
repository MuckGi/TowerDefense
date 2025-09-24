using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading;

public class PlayerHp : MonoBehaviour
{  
    [SerializeField] private float maxHp = 20;
    [SerializeField] private GameObject gameOverUI;

    private float currentHp;
    private GameOverUIManager gameOverUIManager;
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public Image imageScreenRed;

    private void Awake()
    {
        currentHp = maxHp;
    }
    private void Start()
    {
        gameOverUIManager = gameOverUI.GetComponent<GameOverUIManager>();
    }
    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHp <= 0)
        {
            GameOver();
        }
    }
    private IEnumerator HitAlphaAnimation()
    {
        imageScreenRed.color = new Color(1, 0, 0, 0.6f); 

        float timer = 0f;
        float duration = 0.3f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; 
            float alpha = Mathf.Lerp(0.6f, 0f, timer / duration);
            imageScreenRed.color = new Color(1, 0, 0, alpha);
            yield return null;
        }
    }
    private void GameOver()
    {
        if(gameOverUIManager != null)
        {
            gameOverUIManager.TriggerGameOver();
        }
    }
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
