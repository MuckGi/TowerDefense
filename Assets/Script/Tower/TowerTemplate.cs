using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;
    public GameObject followTowerPrefab;
    public GameObject[] boomerangProjectilePrefabs;
    public Weapon[] weapon;

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;
        public float damage;
        public float slow;
        public float buff;
        public float bank;
        public float rate;
        public float rateBuff;
        public float range;
        public int cost;
        public int sell;
    }
}
