using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;

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
    private SpawnManager _spawnManagerScript;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player is null");
        }
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
        _thisCollider= GetComponent<BoxCollider2D>();
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
        _shootingOn = true;

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
            _fireRate = Random.Range(3f, 7f);
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
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6.5)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 8, 0);
        }

    }
    public void AvoidShot()
    {
        int move = Random.Range(0, 2);
        float t = 1;
        Vector2 _currentPosition = transform.position;
        Vector2 _movedPositionLeft = new Vector2(transform.position.x - 2, transform.position.y);
        Vector2 _movedPositionRight = new Vector2(transform.position.x + 2, transform.position.y);

        if (move == 0)
        {
            transform.position = Vector2.Lerp(_currentPosition, _movedPositionLeft, t);
        }
        else
        {
            transform.position = Vector2.Lerp(_currentPosition, _movedPositionRight, t);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null )
            {
                player.Damage();
            }
            Destroy(_thisCollider);
            _animator.SetTrigger("OnEnemyDeath");
            _speed= 0;
            _enemyaudioSource.Play();
            _shootingOn = false;
            _spawnManagerScript.EnemyDestroyed(1);
            Destroy(this.gameObject, 2f);  
        }
        else if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            if (_player != null)
            {
                Destroy(_thisCollider);
                _animator.SetTrigger("OnEnemyDeath");
                _speed = _speed / 3;
                _enemyaudioSource.Play();
                _shootingOn = false;
                _spawnManagerScript.EnemyDestroyed(1);
                Destroy(this.gameObject, 2f); 
            }  
        }
        else if (other.tag == "Blaston")
        {
            if (_player != null)
            {
                Destroy(_thisCollider);
                _animator.SetTrigger("OnEnemyDeath");
                _speed = _speed / 3;
                _enemyaudioSource.Play();
                _shootingOn = false;
                _spawnManagerScript.EnemyDestroyed(1);
                Destroy(this.gameObject, 2f);
            }
        }
    }
}
