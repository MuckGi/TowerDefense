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

  
    public void Setup(Transform target, float dmg, float speedRate, float range)
    {
        targetEnemy = target;
        damage = dmg;
        // 공속(Rate)의 역수를 이동 속도로 사용하여 타워 성능을 반영
        moveSpeed = 1f / speedRate * 5f; // 기본 속도 보정값(5f)을 곱해 실제 이동 속도 설정
        maxRange = range;

        startPosition = transform.position;
    }

    void Update()
    {
        // 1. 타워로 돌아가는 중이 아닐 때 (적에게 날아가는 중)
        if (!returning)
        {
            Vector3 targetPosition = (targetEnemy != null) ? targetEnemy.position : transform.position + transform.right * maxRange;
            Vector3 direction = (targetPosition - transform.position).normalized;

            // 이동
            transform.position += direction * moveSpeed * Time.deltaTime;
            traveledDistance += moveSpeed * Time.deltaTime;

            // 최대 사거리에 도달했거나 타겟을 놓쳤을 경우 돌아오기 시작
            if (traveledDistance >= maxRange || targetEnemy == null)
            {
                returning = true;
            }
        }
        // 2. 타워로 돌아가는 중일 때
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

        // 부메랑이 회전하는 시각적 효과 추가 (옵션)
        transform.Rotate(Vector3.forward * 720f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 적에게 데미지 적용
        if (other.CompareTag("Enemy"))
        {
            EnemyHp enemyHp = other.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                // [TODO] 부메랑은 다회 타격이 가능하므로, 쿨타임 또는 타격 횟수 제한 로직이 추가되어야 함
                enemyHp.TakeDamage(damage);
            }
        }
    }
}