using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]private int currentGold = 150;

    public int CurrentGold
    {
        set => currentGold = Mathf.Max(0, value);
        get => currentGold;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            currentGold += 100;
        }
    }
}
