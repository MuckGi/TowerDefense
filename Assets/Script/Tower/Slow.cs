using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;

    private void Awake()
    {
        towerWeapon = GetComponentInParent<TowerWeapon>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy"))
        {
            return;
        }
        Movement movement = collision.GetComponent<Movement>();
        movement.MoveSpeed -= movement.MoveSpeed * towerWeapon.Slow;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
     if(!collision.CompareTag("Enemy"))
        {
            return;
        }
        collision.GetComponent<Movement>().ResetMoveSpeed();
    }
}
