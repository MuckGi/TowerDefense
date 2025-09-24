using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("TowerDefense");
    }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
