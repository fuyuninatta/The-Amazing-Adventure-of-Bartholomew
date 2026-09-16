using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SummonEnemy : MonoBehaviour
{
    public static SummonEnemy instance;
    public List<GameObject> EnemyPrefabs;
    public List<Transform> spawnPoints;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnEnemy()
    {
        if (EnemyPrefabs == null || EnemyPrefabs.Count == 0 || spawnPoints == null || spawnPoints.Count == 0)
        {
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            //choose random enemy
            int randomEnemyIndex = Random.Range(0, EnemyPrefabs.Count);
            //add random offset for position
            float offsetRange = 5.0f;
            Vector3 randomOffset = new Vector3(Random.Range(-offsetRange, offsetRange),0,Random.Range(-offsetRange, offsetRange));

            //spawn all enemies
            GameObject SpawnedEnemy = Instantiate(EnemyPrefabs[randomEnemyIndex], point.position + randomOffset, point.rotation, point);
            SpawnedEnemy.GetComponent<EnemyController>().SetAlwaysChase();

            //summon effect
            Instantiate(SpawnBoss.instance.SummonEffect, point.position + randomOffset, point.rotation, point);
        }
    }

    public void DestroyEnemy()
    {
        foreach(Transform point in spawnPoints)
        {
            Destroy(point.gameObject);
        }
    }
}
