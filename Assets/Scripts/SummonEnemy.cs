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
            //spawn all enemies
            Instantiate(EnemyPrefabs[randomEnemyIndex], point.position, point.rotation);
        }
    }
}
