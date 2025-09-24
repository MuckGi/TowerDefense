using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    public GameObject gameOverText;
    public GameObject gameOverPanel;
    public Animator gameOverAnimator;             
    public CanvasGroup buttonGroup;              

    public float dropInDuration = 0.5f;          
    public float delayAfterDropIn = 0.3f;        
    public float fadeDuration = 0.5f;            

    private void Start()
    {        
        gameOverAnimator.enabled = false;

        buttonGroup.alpha = 0f;
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;
            
    }  

    public void TriggerGameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverText.SetActive(true);
        gameOverAnimator.enabled = true;
        gameOverAnimator.Play("DropIn", 0, 0f);
        Time.timeScale = 0f;

        StartCoroutine(FadeInButtonsWithDelay());
    }

    private IEnumerator FadeInButtonsWithDelay()
    {

        yield return new WaitForSecondsRealtime(dropInDuration + delayAfterDropIn);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            buttonGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        buttonGroup.alpha = 1f;
        buttonGroup.interactable = true;
        buttonGroup.blocksRaycasts = true;
    }
    public void OnClickRetry()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

