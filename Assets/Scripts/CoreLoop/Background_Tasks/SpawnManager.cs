using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _ammoPrefab;
    [SerializeField]
    private GameObject _healthPackPrefab;

    //waves handlers
    [SerializeField]
    private bool _waveIsActive;
    [SerializeField]
    private int _currentWave;
    [SerializeField]
    private int _maxWaves;
    [SerializeField]
    private int _enemiesToSpawn;
    [SerializeField]
    private int _rareEnemiesToSpawn;
    [SerializeField]
    private int _enemiesToKill;

    [SerializeField]
    private GameObject[] _commonEnemies;
    [SerializeField]
    private GameObject[] _rareEnemies;
    [SerializeField]
    private GameObject _finalBoss;
    private WaitForSeconds _waveBrake = new WaitForSeconds(6);

    [SerializeField]
    private Player _player;

    private bool _stopSpawning = false;

    private UIManager _uiManager;
    private int _destroyed;

    void Start()
    {
       _waveIsActive = false;
       _currentWave =0;
       _maxWaves =5;
        _destroyed = 0;

    }

    public void StartSpawning()
    {
        StartCoroutine(RareEnemyWaveSpawner());
        StartCoroutine(EnemyWaveSpawner());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnAmmoRoutine());
        StartCoroutine(SpawnHealthPackRoutine());

        _uiManager = GameObject.Find("Canvass").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI is null");
        }
    }
    #region Coroutines
    IEnumerator EnemyWaveSpawner()
    {
        _enemiesToSpawn = 6;
        _rareEnemiesToSpawn = 3;
        _waveIsActive = true;
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false && _waveIsActive)
        {
            
            for (int currentWave = 1; currentWave <= _maxWaves; currentWave++)
            {
                Debug.Log("Current wave: " + currentWave.ToString());
                _enemiesToKill = _enemiesToSpawn + _rareEnemiesToSpawn;
                _uiManager.NewTakedown(_enemiesToKill);
                _currentWave = currentWave;
                for (int enemiesOn = 0; enemiesOn < _enemiesToSpawn; enemiesOn++)
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-8.5f, 8.5f), 8, 0);
                    int randomCommonEnemy = Random.Range(0, _commonEnemies.Length);
                    GameObject newEnemy = Instantiate(_commonEnemies[randomCommonEnemy], posToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    yield return new WaitForSeconds(4f);
                }
                while (_enemiesToKill >= 1)
                {
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("End of common Wave");
                _enemiesToSpawn = _enemiesToSpawn + 2;
                _player.AddMaxAmmo();
                yield return _waveBrake;

                if (currentWave == 5 && _enemiesToKill == 0)
                {
                    SpawnFinalBoss();
                    StopCoroutine(RareEnemyWaveSpawner());
                    StopCoroutine(EnemyWaveSpawner());
                    StopCoroutine(SpawnHealthPackRoutine());
                }
            }
            _waveIsActive = false;
        }
    }
    IEnumerator RareEnemyWaveSpawner()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false && _waveIsActive)
        {

            for (int currentWave = 1; currentWave <= _maxWaves; currentWave++)
            {
                Debug.Log("Start Rare WAVE: " + currentWave.ToString());
                for (int rareEnemiesOn = 0; rareEnemiesOn < _rareEnemiesToSpawn; rareEnemiesOn++)
                {
                    Debug.Log(_rareEnemiesToSpawn.ToString() + " inside rarespawner");
                    Vector3 posToSpawnRare = new Vector3(Random.Range(-5.0f, 5.0f), 6, 0);
                    int randomRareEnemy = Random.Range(0, _rareEnemies.Length);
                    if (randomRareEnemy == 0)
                    {
                        GameObject newRareEnemy = Instantiate(_rareEnemies[randomRareEnemy], posToSpawnRare, Quaternion.Euler(0, 0, 45));
                        newRareEnemy.transform.parent = _enemyContainer.transform;
                    }
                    else if (randomRareEnemy == 1)
                    {
                        GameObject newRareEnemy = Instantiate(_rareEnemies[randomRareEnemy], posToSpawnRare, Quaternion.Euler(0, 0, 0));
                        newRareEnemy.transform.parent = _enemyContainer.transform;
                    }
                    yield return new WaitForSeconds(8f);
                }
                Debug.Log("End of Rare Spawning");
                while (_enemiesToKill >= 1)
                {
                    yield return new WaitForEndOfFrame();
                }
                _rareEnemiesToSpawn = _rareEnemiesToSpawn + 1;
                Debug.Log("End of Rare Wave");
                yield return _waveBrake;
            }
        }
    }
    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        while (_stopSpawning == false)
        {
            float spawnRandomTime = Random.Range(10.0f, 15.0f);
            Vector3 posToSpawn = new Vector3(Random.Range(-7f, 7f), 7, 0);
            int randomPowerUp = Random.Range(0, _powerups.Length);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
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
            float spawnRandomTime = Random.Range(25.0f, 35.0f);
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            Instantiate(_healthPackPrefab, posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(spawnRandomTime);
        }
    }

    void SpawnFinalBoss()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        Instantiate(_healthPackPrefab, posToSpawn, Quaternion.identity);
        Instantiate(_powerups[2], posToSpawn, Quaternion.identity);

        _finalBoss.gameObject.SetActive(true);
    }
    #endregion
    public void EnemyDestroyed(int points)
    {
        _enemiesToKill -= points;
        _destroyed = _enemiesToKill;
        _uiManager.NewTakedown(_destroyed);
        
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
