using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f;
    [SerializeField] private bool loop = true;
    [SerializeField] private float resetPositionX = -20f;
    [SerializeField] private float startPositionX = 20f;
                
    void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if(loop&& transform.position.x < resetPositionX)
        {
            Vector3 pos = transform.position;
            pos.x = startPositionX;
            transform.position = pos;
        }
    }
}
