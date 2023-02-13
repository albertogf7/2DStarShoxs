using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _tripleShotPowerUpPrefab;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private GameObject _ammoPrefab;
    [SerializeField]
    private GameObject _healthPackPrefab;

    private bool _stopSpawning = false;

    void Start()
    {
       
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnAmmoRoutine());
        StartCoroutine(SpawnHealthPackRoutine());
    }
    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2f);
        }
    }
    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        while (_stopSpawning == false)
        {
            float spawnRandomTime = Random.Range(10.0f, 15.0f);
            Vector3 posToSpawn = new Vector3(Random.Range(-7f, 7f), 7, 0);
            int randomPowerUp = Random.Range(0, powerups.Length);
            Instantiate(powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(spawnRandomTime);
        }
    }
    IEnumerator SpawnAmmoRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        while (_stopSpawning == false)
        {
            float spawnRandomTime = Random.Range(10.0f, 20.0f);
            Vector3 posToSpawn = new Vector3(Random.Range(-7f, 7f), 7, 0);
            Instantiate(_ammoPrefab, posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(spawnRandomTime);
        }
    }
    IEnumerator SpawnHealthPackRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        while (_stopSpawning == false)
        {
            float spawnRandomTime = Random.Range(20.0f, 30.0f);
            Vector3 posToSpawn = new Vector3(Random.Range(-7f, 7f), 7, 0);
            Instantiate(_healthPackPrefab, posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(spawnRandomTime);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
