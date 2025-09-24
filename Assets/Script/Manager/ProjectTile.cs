using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectTile : MonoBehaviour
{
    private Movement movement;
    private Transform target;
    private float damage;

    public void Setup(Transform target, float damage)
    {
        movement = GetComponent<Movement>();
        this.target = target;
        this.damage = damage;
    }
    private void Update()
    {
        if(target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            movement.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        if (collision.transform != target) return;

        //collision.GetComponent<Enemy>().OnDie();
        collision.GetComponent<EnemyHp>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
