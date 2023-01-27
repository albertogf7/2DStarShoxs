using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    
    [SerializeField]
    private int _currentHealth;
    [SerializeField] 
    private int _destroyed;

    [SerializeField]
    private float _speed = 7.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private float _attackDelay = 0.25f;

    [SerializeField]
    private GameObject _laserPrefab;
    private bool _laserCanFire;

    private SpawnManager _spawnManager;
    private bool _tripleShotEnabled = false;
    [SerializeField]
    private bool _BoostEnabled = false;
    [SerializeField]
    private bool _ShieldEnabled = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldGameObj;
    [SerializeField]
    private GameObject _fuegito;
    [SerializeField]
    private GameObject _wFR;
    [SerializeField]
    private GameObject _wFL;
    [SerializeField]
    private GameObject _explosionFire;

    private UIManager _uiManager;
    private GameManager _gameManager;

    [SerializeField]
    private AudioClip _laserSound;
    [SerializeField]
    private AudioClip _explosionClip;
    [SerializeField]
    private AudioClip _powerUpClip;


    private AudioSource _audioSource;

    void Awake()
    {
        transform.position = new Vector3(0, 0, 0);

        _laserCanFire = true;

        _currentHealth = 3;

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI is null");
        }
        _wFL.SetActive(false);
        _wFR.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("Audiosource player missing");
        }
        else
        {
            _audioSource.clip = _laserSound;
            _audioSource.pitch = 0.9f;
            _audioSource.volume = 0.7f;
        }

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager NOT found");
        }

    }

    void Update()
    {
        CalculateMovement();
        ShootLaser();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 1f), 0);

        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void ShootLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _laserCanFire && _tripleShotEnabled)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _audioSource.Play();
            _laserCanFire = false;
            StartCoroutine(LaserReloading());
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _laserCanFire)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _audioSource.Play();
            _laserCanFire = false;
            StartCoroutine(LaserReloading());
        }
    }
    IEnumerator LaserReloading()
    {
        yield return new WaitForSeconds(_attackDelay);
        _laserCanFire = true;
    }
    public void Damage()
    {
        if (_ShieldEnabled == true)
        {
            _ShieldEnabled = false;
            StartCoroutine(ShieldTurnOff());
            _uiManager.NewTakedown(1);
            return;
        }
        else if (_ShieldEnabled == false)
        {
            _currentHealth --;
            _uiManager.UpdateLives(_currentHealth);

            if (_currentHealth == 2)
            {
                _wFR.SetActive(true);
            }

            if (_currentHealth == 1)
            {
                _wFL.SetActive(true);
            }

            if (_currentHealth < 1)
            {
                _spawnManager.OnPlayerDeath();
                _gameManager.GameOver();
                _uiManager.GameOver();
                Instantiate(_explosionFire, transform.position, Quaternion.identity);
                _audioSource.clip = _explosionClip;
                _audioSource.pitch = 0.8f;
                _audioSource.volume = 1f;
                _audioSource.Play();
                Destroy(this.gameObject);
            }
        }
    }
    public void ActivateShield()
    {
        _ShieldEnabled = true;
        _shieldGameObj.SetActive(true);
    }

    IEnumerator ShieldTurnOff()
    {
        _shieldGameObj.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        _shieldGameObj.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _shieldGameObj.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        _shieldGameObj.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _shieldGameObj.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        _shieldGameObj.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        _shieldGameObj.SetActive(false);
    }
    public void ActivateTripleShot()
    {
        _audioSource.pitch = 1.2f;
        _audioSource.volume = 1f;
        StartCoroutine(TripleShotActive());
    }

    IEnumerator TripleShotActive()
    {
        _tripleShotEnabled = true;
        yield return new WaitForSeconds(5.0f);
        _audioSource.pitch = 0.9f;
        _audioSource.volume = 0.7f;
        _tripleShotEnabled = false;
    }

    public void ActivateBoost()
    {
        _BoostEnabled = true;
        StartCoroutine(BoostActive());
    }

    IEnumerator BoostActive()
    {
        if (_BoostEnabled)
        {
            _fuegito.SetActive(true);
            _speed *= _speedMultiplier;
            yield return new WaitForSeconds(6.0f);
            _speed /= _speedMultiplier;
            _BoostEnabled = false;
            _fuegito.SetActive(false);
        }
    }

    public void AddDestroyed(int points)
    {
        _destroyed += points;
        _uiManager.NewTakedown(_destroyed); 
    }
}

/* 
 [SerializeField]
  private AudioClip[] clipsTrack1;
[SerializeField]
  private AudioClip[] clipsTrack2;
[SerializeField]
  private AUDIOClip[] clipsTrack3;
[SerializeField]
  private AudioClip[] clipsTrack4;
[SerializeField]
  private AudioClip[] clipsTrack5;
[SerializeField]
  private AUDIOClip[] clipsTrack6;

 change laser sound to one of the current-track's array of clips through powerUps;
 */
