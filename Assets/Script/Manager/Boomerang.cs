using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
   
    private Transform targetEnemy;
    private float damage;
    private float moveSpeed;    
    private float maxRange;     

    private Vector3 startPosition; 
    private bool returning = false; 
    private float traveledDistance = 0f; 
  
    private float hitCooldown = 0.1f;
    private Dictionary<EnemyHp, float> lastHitTime = new Dictionary<EnemyHp, float>();


    public void Setup(Transform target, float dmg, float speedRate, float range)
    {
        targetEnemy = target;
        damage = dmg;               
        moveSpeed = 1f / speedRate * 5f; 
        maxRange = range;

        startPosition = transform.position;
    }

    void Update()
    {       
        if (!returning)
        {
            Vector3 targetPosition = (targetEnemy != null) ? targetEnemy.position : transform.position + transform.right * maxRange;
            Vector3 ToTarget = targetPosition - transform.position;

            if(ToTarget.sqrMagnitude < 0.0001f)
            {
                returning = true;
                transform.Rotate(Vector3.forward * 720f * Time.deltaTime);
                return;
            }
            Vector3 direction = ToTarget.normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            traveledDistance += moveSpeed * Time.deltaTime;
           
            if (traveledDistance >= maxRange )
            {
                returning = true;
            }
        }
        else
        {
            Vector3 returnDirection = (startPosition - transform.position).normalized;
            transform.position += returnDirection * moveSpeed * Time.deltaTime;

            // 타워 위치(시작 지점)에 거의 도달했을 경우 파괴
            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }

        transform.Rotate(Vector3.forward * 720f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.CompareTag("Enemy"))
        {
            EnemyHp enemyHp = other.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {     
                if (lastHitTime.ContainsKey(enemyHp))
                {
                    if (Time.time <lastHitTime[enemyHp] + hitCooldown)
                    {
                        return;
                    }                    
                }                
                enemyHp.TakeDamage(damage);
                lastHitTime[enemyHp] = Time.time;
            }
        }
    }
}