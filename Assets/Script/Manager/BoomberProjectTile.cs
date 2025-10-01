using UnityEngine;

public class BoomberProjectile : MonoBehaviour
{
    // TowerWeapon에서 받아올 데이터
    private Transform targetEnemy;
    private float damage;
    private float explosionRadius;
    private GameObject explosionEffectPrefab;
    private LayerMask targetLayer;

    // 투사체 이동 속도 (템플릿에서 로드할 수 있음)
    private float moveSpeed = 7f;

    // TowerWeapon에서 호출될 Setup 함수
    public void Setup(Transform target, float dmg, float radius, GameObject effectPrefab, LayerMask layer)
    {
        targetEnemy = target;
        damage = dmg;
        explosionRadius = radius;
        explosionEffectPrefab = effectPrefab;
        targetLayer = layer;
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            // 타겟을 향해 부드럽게 이동 (투사체 로직과 유사)
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 타겟이 사거리를 벗어나거나 파괴되면
            // 타겟의 마지막 위치로 계속 이동하거나, 도중에 Explode()를 호출하도록 설정할 수 있음
        }
        else
        {
            // 타겟이 사라졌을 경우 즉시 Explode() 또는 일정 시간 후 파괴
            Explode();
        }
    }

    private void Explode()
    {
        // 1. 폭발 이펙트 생성 (프리팹은 자동 파괴 로직이 있어야 함)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. 폭발 범위 내의 적 탐색
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);

        // 3. 탐색된 적들에게 피해 적용
        foreach (Collider2D enemyCollider in enemies)
        {
            EnemyHp enemyHp = enemyCollider.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                // 광역 피해 적용 (광역 피해는 다른 타워보다 데미지가 낮게 설정될 수 있음)
                enemyHp.TakeDamage(damage);
            }
        }

        // 4. 자신을 파괴
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 적에 닿았을 때 Explode() 호출
        if (other.CompareTag("Enemy"))
        {
            Explode();
        }
    }
}