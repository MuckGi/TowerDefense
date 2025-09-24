using System.Collections;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff, Boomerang , Multy, }
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, TryAttackboomerang, TryAttackMulty,}

public class TowerWeapon : MonoBehaviour
{

    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate;    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WeaponType weaponType;

    [Header("Cannon")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform hitEffect;
    [SerializeField] private LayerMask targetLayer;

    [Header("Boomerang")]
    [SerializeField] private GameObject boomerangTemplate;
    [SerializeField] private Transform boomerangSpawnPoint;    

    [Header("Multy")]
    [SerializeField] private GameObject multyProjectilePrefab;
    [SerializeField] private Transform[] multySpawnPoints;

    [Header("Machine gun")]

    [Header("Boomber")]

    [Header("Sniper")]

    [Header("BankTower")]


    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private SpriteRenderer spriteRenderer;
    private EnemySpawner enemySpawner;
    private TowerSpawner towerSpawner;
    private PlayerGold playerGold;
    private Tile ownerTile;   

    private float addedDamage;
    private int buffLevel;
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int UpgradeCost => Level <MaxLevel ? towerTemplate.weapon[level+1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public int Level => level +1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
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

        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
                
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
        
    }
    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());
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
                if(weaponType ==WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if(weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
                else if(weaponType == WeaponType.Boomerang)
                {
                    ChangeState(WeaponState.TryAttackboomerang);
                }
                else if(weaponType == WeaponType.Multy)
                {
                    ChangeState(WeaponState.TryAttackMulty);
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
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
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

            yield return null;
        }
    }
    private IEnumerator TryAttackboomerang()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnBooemrang();            
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
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
            for(int i =0; i< multySpawnPoints.Length; ++i)
            {
                GameObject clone = Instantiate(multyProjectilePrefab, multySpawnPoints[i].position, Quaternion.identity);
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                clone.GetComponent<ProjectTile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
            }            
        }
    }
    private void SpawnBooemrang()
    {
        GameObject clone = Instantiate(boomerangTemplate, boomerangSpawnPoint.position, Quaternion.identity);
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<ProjectTile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }
    public void OnBuffArounTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        for (int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if(weapon.BuffLevel > Level)
            {
                continue;
            }
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                if(weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    weapon.BuffLevel = Level;
                }
                
            }
        }
    }
    private Transform FindClosestAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;
        for(int i = 0; i< enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            if(distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }
        return attackTarget;
    }
    private bool IsPossibleToAttackTarget()
    {
        if(attackTarget == null)
        {
            return false;
        }

        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if(distance > towerTemplate.weapon[level].range )
        {
            attackTarget = null;
            return false;
        }
        return true;
    }
    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<ProjectTile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
        
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
        if(playerGold.CurrentGold < towerTemplate.weapon[level+1].cost)
        {
            return false;
        }
    
        level++;        
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
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

        TryRotateHeadIfNeeded();

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

        if (towerTemplate.name.Contains("Tower03")|| towerTemplate.name.Contains("Tower04"))
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
