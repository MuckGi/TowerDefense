using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float moveDuration = 2f;

    private float timer = 0f;

    void Start()
    {
        if (startPoint != null && endPoint != null)
        {
            transform.position = startPoint.position;
        }
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / moveDuration);
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);

        if (t >= 1f)
        {
            Destroy(gameObject); 
        }
    }
}
