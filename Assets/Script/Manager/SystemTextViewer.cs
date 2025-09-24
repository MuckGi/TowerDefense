using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SystemType { Money = 0, Build }
public class SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI textSystem;
    private TMPAlpha tmpAlpha;

    private void Awake()
    {
        textSystem = GetComponent<TextMeshProUGUI>();
        tmpAlpha = GetComponent<TMPAlpha>();
    }

    public void PrintText(SystemType type)
    {
        switch(type)
        {
            case SystemType.Money:
                textSystem.text = "Not enough money.";
                break;
            case SystemType.Build:
                textSystem.text = "Invalid bulid tower.";
                break;
        }
        tmpAlpha.FadeOut();
    }
}
