using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMoving : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2f;

    private Player _player;
    private Animator _animator;
    private BoxCollider2D _thisCollider;

    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _enemyaudioSource;

    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private bool _shootingOn;
    [SerializeField]
    private GameObject _enemyLasers;
    [SerializeField]
    private int _specialAbilityID;
    [SerializeField]
    private bool _shieldIsOn;
    [SerializeField]
    private GameObject _enemyShield;
    [SerializeField]
    private SpriteRenderer _enemShieldRenderer;
    [SerializeField]
    private GameObject _homingBeam;
    private HomingBeam _homingBeamScript;

    [SerializeField]
    private float _movementFrequency = 1.5f;
    [SerializeField]
    private float _movementMagnitude = 6f;
    [SerializeField]
    private int _offset;
    private SpawnManager _spawnManagerScript;

    private void Start()
    {
        #region FindComponents
       /* _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player is null");
        }*/
        _spawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManagerScript == null)
        {
            Debug.Log("SpawnManager is null");
        }
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.Log("Animator is null");
        }
        _thisCollider = GetComponent<BoxCollider2D>();
        if (_thisCollider == null)
        {
            Debug.Log("collider is null");
        }
        _enemyaudioSource = GetComponent<AudioSource>();
        if (_enemyaudioSource == null)
        {
            Debug.LogError("Audiosource player missing");
        }
        else
        {
            _enemyaudioSource.clip = _explosionSound;
        }
        _homingBeamScript = _homingBeam.GetComponentInChildren<HomingBeam>();
        if (_homingBeamScript == null)
        {
            Debug.LogError("Homing Beam Script missing");
        }
        #endregion


        _shootingOn = true;
        _specialAbilityID = Random.Range(0, 2);
        _shieldIsOn = false;

        switch (_specialAbilityID)
        {
            case 0:
                _offset = Random.Range(-2, 2);
                ShieldOn();
                break;
            case 1:
                _offset = Random.Range(-6, 6);
                _homingBeam.gameObject.SetActive(true);
                break;
        }
    }
    void Update()
    {
        CalculateMovement();
        ShootingLoop();
    }

    void ShootingLoop()
    {
        if (Time.time > _canFire && _shootingOn)
        {
            _fireRate = Random.Range(2f, 4f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_enemyLasers, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        if (_shootingOn)
        {
            float x = (Mathf.Cos(Time.time * _movementFrequency) * _movementMagnitude) + _offset;
            float y = transform.position.y - (_speed * Time.deltaTime);
            float z = transform.position.z;

            transform.position = new Vector3(x, y, z);
        }
        if (transform.position.y < -6.5)
        {
            EnemyBSpecials();
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 8, 0);
        }
    }

    void EnemyBSpecials()
    {
        switch (_specialAbilityID)
        {
            case 0:
                ShieldOn();
                break;
            case 1:
                if (_homingBeam != null)
                {
                    _homingBeam.transform.SetParent(null);
                    Debug.Log("Homing unparented");
                }
                ShootHomingLaser();
                break;
        }
    }
    private void ShieldOn()
    {
        _enemShieldRenderer.color = Color.red;
        _enemyShield.gameObject.SetActive(true);
        _shieldIsOn = true;
    }

    private void ShootHomingLaser()
    {
        
        _homingBeamScript.ActivateSeeking();
        _specialAbilityID = 0;
    }
    private void OnEnemyDeath()
    {
        Destroy(_thisCollider);
        _animator.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _enemyaudioSource.Play();
        _shootingOn = false;
        _spawnManagerScript.EnemyDestroyed(1);
        Destroy(this.gameObject, 2f);
        Destroy(_homingBeam.gameObject);
        Destroy(_enemyShield.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_shieldIsOn && other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            player.Damage();
            _enemyShield.gameObject.SetActive(false);
            _shieldIsOn = false;
        }
        else if (_shieldIsOn && other.tag == "Blaston")
        {
            _enemyShield.gameObject.SetActive(false);
            _shieldIsOn = false;
        }

        else if (_shieldIsOn && other.tag == "Laser")
        {
            {
                other.gameObject.SetActive(false);
                _enemyShield.gameObject.SetActive(false);
                _shieldIsOn = false;
            }
        }
        else if (other.tag == "Player" && !_shieldIsOn)
        {
            Player player = other.transform.GetComponent<Player>();

            if (_spawnManagerScript != null)
            {
                player.Damage();
                OnEnemyDeath();
            }
        }
        else if (other.tag == "Laser" && !_shieldIsOn)
        {
            other.gameObject.SetActive(false);
            if (_spawnManagerScript != null)
            {
                OnEnemyDeath();
            }
        }

        else if (other.tag == "Blaston" && !_shieldIsOn)
        {
            if (_spawnManagerScript != null)
            {
                OnEnemyDeath();
            }
        }
        else if (other.tag == "Homing Missile")
        {
            HomingMissile hmScript = other.GetComponent<HomingMissile>();
            hmScript.OriginalPosition();
            OnEnemyDeath();
        }
    }
}
