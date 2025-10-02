using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public enum WeaponType { Cannon = 0, Multy, Boomerang, Laser, MachinGun, Boomber, Sniper, Bank, Slow, Buff,}
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackMulty, TryAttackBoomerang, TryAttackLaser, TryAttackMachinGun, TryAttackBoomber, TryAttackSniper,}

public class TowerWeapon : MonoBehaviour
{

    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate;    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WeaponType weaponType;

    [Header("Cannon & Machin Gun & Sniper")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Multy")]
    [SerializeField] private GameObject multyProjectilePrefab;
    [SerializeField] private Transform[] multySpawnPoints;

    [Header("Boomerang")]    
    [SerializeField] private Transform boomerangSpawnPoint;

    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform hitEffect;
    [SerializeField] private LayerMask targetLayer;
    
    [Header("Boomber")]
    [SerializeField] private GameObject boomberProjectilePrefab;
    [SerializeField] private GameObject boomerExplosionPrefab;

    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private SpriteRenderer spriteRenderer;
    private EnemySpawner enemySpawner;
    private TowerSpawner towerSpawner;
    private PlayerGold playerGold;
    private Tile ownerTile;

    private float addedDamage;
    private float addedRate;
    private int buffLevel;

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage + AddedDamage;
    public float Rate => towerTemplate.weapon[level].rate - AddedRate;
    public float Range => towerTemplate.weapon[level].range;
    public int UpgradeCost => Level <MaxLevel ? towerTemplate.weapon[level+1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public int Level => level +1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public float Bank => towerTemplate.weapon[level].bank;
    public float RatebuffValue => towerTemplate.weapon[level].rateBuff;

    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public float AddedRate
    {
        set => addedRate = Mathf.Max(0, value);
        get => addedRate;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0,value);
        get => buffLevel;
    }
    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold,Tile ownerTile)
    {
                
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser || weaponType == WeaponType.Multy || weaponType == WeaponType.Boomerang || weaponType == WeaponType.MachinGun || weaponType == WeaponType.Boomber || weaponType == WeaponType.Sniper)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
                
        if (weaponType != WeaponType.Bank && weaponType != WeaponType.Slow && weaponType != WeaponType.Buff)
        {
            ChangeState(WeaponState.SearchTarget);
        }
        
    }
    public void ChangeState(WeaponState newState)
    {
        StopAllCoroutines();
        weaponState = newState;
        StartCoroutine(weaponState.ToString());
    }
    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }
    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }
    private IEnumerator SearchTarget()
    {
        while(true)
        {           
            attackTarget = FindClosestAttackTarget();

            if(attackTarget != null)
            {
                if(weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if (weaponType == WeaponType.Multy)
                {
                    ChangeState(WeaponState.TryAttackMulty);
                }
                else if (weaponType == WeaponType.Boomerang)
                {
                    ChangeState(WeaponState.TryAttackBoomerang);
                }
                else if(weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
                else if (weaponType == WeaponType.MachinGun)
                {
                    ChangeState(WeaponState.TryAttackMachinGun);
                }                             
                else if (weaponType == WeaponType.Boomber)
                {
                    ChangeState(WeaponState.TryAttackBoomber);
                }
                else if (weaponType == WeaponType.Sniper)
                {
                    ChangeState(WeaponState.TryAttackSniper);
                }                
            }            
                yield return null;
        }
    }
    private IEnumerator TryAttackCannon()
    {
        while(true)
        {           
           if(IsPossibleToAttackTarget()== false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            yield return new WaitForSeconds(Rate);

            SpawnProjectile(projectilePrefab, spawnPoint, Damage);
        }
    }
    private IEnumerator TryAttackLaser()
    {
        EnableLaser();

        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            SpawnLaser();

            yield return new WaitForSeconds(Rate);
        }
    }
    private IEnumerator TryAttackBoomerang()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            yield return new WaitForSeconds(Rate);
            SpawnBoomerangProjectile();
        }
    }
    private IEnumerator TryAttackMulty()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(Rate);

            StartCoroutine(ShootMultyBurst());
        }
    }
    private IEnumerator TryAttackMachinGun()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            StartCoroutine(ShootMachinGunBurst());
            yield return new WaitForSeconds(Rate);            
        }
    }
    private IEnumerator TryAttackBoomber()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(Rate);
            SpawnBoomberProjectile();
        }
    }
    private IEnumerator TryAttackSniper()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(Rate); 
            SpawnProjectile(projectilePrefab, spawnPoint, Damage);
        }
    }   
    private IEnumerator ShootMultyBurst()
    {
        int burstCount = 3;
        
        for(int i = 0; i < burstCount; i++)
        {
            if (attackTarget == null) break;

            Transform currentSpawnPoint = multySpawnPoints.Length > 0 ? multySpawnPoints[i % multySpawnPoints.Length] : spawnPoint;
            SpawnProjectile(multyProjectilePrefab, currentSpawnPoint, Damage);
            yield return new WaitForSeconds(0.01f);
        }
    }
    private IEnumerator ShootMachinGunBurst()
    {
        int burstCount = 5;
        float shotDelay = 0.05f;

        for(int i = 0; i < burstCount; i++)
        {
            if (attackTarget == null) break;

            SpawnProjectile(projectilePrefab, spawnPoint, Damage);
            yield return new WaitForSeconds(shotDelay);
        }
    }
    private void SpawnBoomerangProjectile()
    {      
        int projectileIndex = level;

        if (towerTemplate.boomerangProjectilePrefabs.Length <= projectileIndex)
        {            
            return;
        }
        GameObject currentPrefab = towerTemplate.boomerangProjectilePrefabs[projectileIndex];
        GameObject clone = Instantiate(currentPrefab, boomerangSpawnPoint.position, Quaternion.identity);

        var boomerang = clone.GetComponent<Boomerang>();
        if (boomerang != null)
        {
            boomerang.Setup(attackTarget, Damage, Rate, Range); 
        }
    }
    private void SpawnBoomberProjectile()
    {
        float explosionRange = 2.0f;
        GameObject clone = Instantiate(boomberProjectilePrefab, spawnPoint.position, Quaternion.identity);

        var boomberProjectile = clone.GetComponent<BoomberProjectile>();
        if(boomberProjectile != null)
        {
            boomberProjectile.Setup(attackTarget, Damage, explosionRange, boomerExplosionPrefab, targetLayer);
        }
    }
    private void SpawnProjectile(GameObject prefab, Transform sp, float damagetoApply)
    {
        GameObject clone = Instantiate(prefab, sp.position, Quaternion.identity);
        var projectile = clone.GetComponent<ProjectTile>();
        if(projectile != null)
        {
            projectile.Setup(attackTarget, damagetoApply);
        }

    }
    public void OnBuffArounTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        float damageBuffRatio = Buff;
        float rateBuffValue = RatebuffValue;

        for (int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if(weapon.BuffLevel > Level)
            {
                continue;
            }
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                if(weapon.WeaponType != WeaponType.Bank && weapon.WeaponType != WeaponType.Buff && weapon.WeaponType != WeaponType.Slow)
                {
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    weapon.AddedRate = rateBuffValue;
                    weapon.BuffLevel = Level;
                }                
            }
        }
    }
    private Transform FindClosestAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;
        Transform closestTarget = null;
        for (int i = 0; i< enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            if(distance <= Range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                closestTarget = enemySpawner.EnemyList[i].transform;
            }
        }
        return closestTarget;
    }
    private bool IsPossibleToAttackTarget()
    {
        if(attackTarget == null)
        {
            return false;
        }
        float distance = Vector3.Distance(attackTarget.position, transform.position);

        if(distance > Range )
        {
            attackTarget = null;
            return false;
        }
        return true;
    }
    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }
    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }
    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].transform == attackTarget)
            {
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;

                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHp>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }    
    public bool Upgrade()
    {
        if(Level >= MaxLevel || playerGold.CurrentGold < towerTemplate.weapon[level+1].cost)
        {
            return false;
        }
    
        level++;        
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser || weaponType == WeaponType.Multy || weaponType == WeaponType.Boomerang || weaponType == WeaponType.MachinGun || weaponType == WeaponType.Boomber || weaponType == WeaponType.Sniper)
        {
            if(spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if( spriteRenderer != null)
            {
                spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
            }
        }

        if (weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
    
        towerSpawner.OnBuffAllBuffTowers();
        return true;
    }
    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        ownerTile.IsBuildTower = false;
        Destroy(gameObject);
    }
    private void LateUpdate()
    {
        TryRotateHeadIfNeeded();
    }
    private void TryRotateHeadIfNeeded()
    {
        if (towerTemplate == null) return;

        if (towerTemplate.name.Contains("Tower08")|| towerTemplate.name.Contains("Tower09")|| towerTemplate.name.Contains("Tower10"))
        {
            Transform head = transform.Find("Head");

            if (head != null)
            {
                head.Rotate(Vector3.forward * 90f * Time.deltaTime);

                SpriteRenderer sr = head.GetComponent<SpriteRenderer>();
                if (sr != null && towerTemplate.weapon.Length > level)
                {
                    sr.sprite = towerTemplate.weapon[level].sprite;
                }
            }
        }
    }
}
