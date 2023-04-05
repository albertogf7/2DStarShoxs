using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ram : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.85f;
    private Player _player;
    private Animator _animator;
    private BoxCollider2D _thisCollider;
    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _enemyaudioSource;
    private SpawnManager _spawnManagerScript;
    [SerializeField]
    private int _ramHealth;
    [SerializeField]
    private bool _playerFound;
    private float _sensorLength = 3.5f;
    [SerializeField]
    private float _distanceToPlayer;
    [SerializeField]
    private float _rammingSpeed = 9f;

    [SerializeField]
    private AudioClip _metalHit;
    [SerializeField]
    private SpriteRenderer _ramRenderer;

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

        _ramHealth = 4;
        _rammingSpeed = 9f;
        _playerFound = false;
        _ramRenderer.color = Color.red;


    }
    void FixedUpdate()
    {
        CalculateMovement();
        LookingForPlayer();
    }

    void CalculateMovement()
    {
        if (!_playerFound)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y < -6.5)
            {
                float randomX = Random.Range(-9f, 9f);
                transform.position = new Vector3(randomX, 8, 0);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _rammingSpeed * Time.deltaTime);
        }
    }

    void DamageRam()
    {
        if (_ramHealth >= 0)
        {
            _ramHealth--;
            AudioSource.PlayClipAtPoint(_metalHit, transform.position);

            if (_ramHealth == 3)
            {
                _ramRenderer.color = Color.blue;
            }
            if (_ramHealth == 2)
            {
                _ramRenderer.color = Color.yellow;
            }
            else if (_ramHealth == 1)
            {
                _ramRenderer.color = Color.white;
            }
            else if (_ramHealth == 0)
            {
                DestroyRam();
            }
        }
    }
    void DestroyRam()
    {
        _ramRenderer.color = Color.black;
        Destroy(_thisCollider);
        _animator.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _enemyaudioSource.Play();
        _spawnManagerScript.EnemyDestroyed(1);
        Destroy(this.gameObject, 0.3f);
    }


    void LookingForPlayer()
    {
        if (_player != null)
        {
            _distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            if (_distanceToPlayer < _sensorLength)
            {
                _playerFound = true;
            }
        } else
        {
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player" && _playerFound)
        {
            if (_player != null)
            {
                _player.Damage();
                _player.Damage();
                DestroyRam();

            }
        }
        else if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            DamageRam();
        }
        else if (other.tag == "Blaston")
        {
            DamageRam();
        }
        else if (other.tag == "Homing Missile")
        {
            HomingMissile hmScript = other.GetComponent<HomingMissile>();
            hmScript.OriginalPosition();
            DestroyRam();
        }
    }
}
