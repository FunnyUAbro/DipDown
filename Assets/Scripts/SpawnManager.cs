using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] enemySpawners;
    [SerializeField] Transform[] boxSpawners;
    [SerializeField] Enemy[] enemyPrefabs;
    [SerializeField] Box boxPrefab;
    [SerializeField] float wavePeriod;
    [SerializeField] int startEnemyAmount;
    [SerializeField] int startBoxAmount;
    float waveTimer;
    int waveCounter;
    int enemiesToSpawn;
    int boxToSpawn;

    void Start()
    {
        waveTimer = wavePeriod;
        enemiesToSpawn = startEnemyAmount;
        boxToSpawn = startBoxAmount;
    }

    void Update()
    {
        waveTimer -= Time.deltaTime;

        if (waveTimer < 0)
        {
            SpawnWave();
        }
    }

    void SpawnWave()
    {

        for (int i = 0; i < enemySpawners.Length; i++)
        {
            if (enemiesToSpawn > 0)
            {
                int prefabIndex = Random.Range(0,enemyPrefabs.Length);

                Instantiate(enemyPrefabs[prefabIndex], enemySpawners[i].position, Quaternion.identity);
                enemiesToSpawn--;

                if (i == enemySpawners.Length - 1 && enemiesToSpawn > 0)
                {
                    i = -1;
                }

            }
            else
                break;
        }

        for (int i = 0; i < boxSpawners.Length; i++)
        {
            if (boxToSpawn > 0)
            {
                Instantiate(boxPrefab, boxSpawners[i].position, Quaternion.identity);
                boxToSpawn--;

                if (i == boxSpawners.Length -1 && boxToSpawn > 0)
                {
                    i = -1;
                }
            }
        }



        waveCounter++;
        boxToSpawn += waveCounter;
        enemiesToSpawn += waveCounter + 1;
        waveTimer = wavePeriod;
    }
}
