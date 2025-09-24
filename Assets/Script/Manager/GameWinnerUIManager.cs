using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinnerUIManager : MonoBehaviour
{
    public static GameWinnerUIManager Instance;
    public GameObject gameClearPanel;
    public Animator textPopAnimator;
    public CanvasGroup buttonGroup;
    public GameObject gameClearText;

    public float textPopDuration = 0.5f;
    public float delayAfterDropIn = 0.3f;
    public float fadeDuration = 0.5f;
    
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    private void Start()
    {
        buttonGroup.alpha = 0f;
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;

        gameClearPanel.SetActive(false);
    }
    public void TriggerGameClear()
    {
        gameClearPanel.SetActive(true);
        gameClearText.SetActive(true);
        Time.timeScale = 0f;

        if (textPopAnimator != null)
        {
            textPopAnimator.Play("TextPop", 0, 0f); 
        }

        StartCoroutine(FadeInButtonsWithDelay());
    }

    private IEnumerator FadeInButtonsWithDelay()
    {
        yield return new WaitForSecondsRealtime(textPopDuration + delayAfterDropIn);

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
