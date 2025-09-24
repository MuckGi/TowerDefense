using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerVisualController : MonoBehaviour
{
    public GameObject headLv1;
    public GameObject headLv2;
    public GameObject headLv3;

    private GameObject currentHead;

    private void Start()
    {
        SetLevel(1);
    }
    public void SetLevel(int level)
    {
        if (currentHead != null)currentHead.SetActive(false);
        switch (level)
        {
            case 1:
                currentHead = headLv1;
                break;
            case 2:
                currentHead = headLv2;
                break;
            case 3:
                currentHead = headLv3;
                break;
            default:
                currentHead = headLv1;
                break;
        }
        if(currentHead != null)
            currentHead.SetActive(true);
    }
    private void Update()
    {
        if(currentHead != null)
            currentHead.transform.Rotate(Vector3.forward * 90f*Time.deltaTime);
    }
}
