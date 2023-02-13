using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private SpawnManager _spawnManager;

    [SerializeField]
    private int _currentHealth;
    [SerializeField] 
    private int _destroyed;

    [SerializeField]
    private float _speed = 7.5f;
    private float _speedMultiplier = 1.8F;

    //Weapon System
    [SerializeField]
    private float _attackDelay = 0.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    private bool _laserCanFire;
    private bool _tripleShotEnabled = false;
    [SerializeField]
    private float _blastonChargeSpeed;
    [SerializeField]
    private float _blastonChargeTime;
    private bool _blastonIsCharging;
    [SerializeField]
    private GameObject _blaston;
    [SerializeField]
    private bool _blastonOnCD;
    private WaitForSeconds _blastonRegen = new WaitForSeconds(10);

    [SerializeField]
    private bool _BoostEnabled = false;
    [SerializeField]
    private bool _ShieldEnabled = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldGameObj;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;
    private int _shieldHealth = 3;

    [SerializeField]
    private Slider _thrusterCDSlider;
    [SerializeField]
    private float _thrusterFillDelaySeconds = 0.5f;
    [SerializeField]
    private float _thrusterCDDelay = 0.3f;
    private WaitForSeconds _thrusterRegen = new WaitForSeconds(5);
    [SerializeField]
    private float _thrusterUsedTime;
    [SerializeField]
    private bool _thrusterIsCharging;
    [SerializeField]
    private bool _thrusterIsBoosting;
    [SerializeField]
    private GameObject _thrusterFire;
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

    public int maxAmmo = 15;
    [SerializeField]
    private int _currentAmmo;
    private bool _isAmmoReady = true;
    [SerializeField]
    private GameObject _ammoIndicator;
    [SerializeField]
    private SpriteRenderer _ammoIndicatorRenderer;

    [SerializeField]
    private CameraShake _camShaker;


    void Awake()
    {
        transform.position = new Vector3(0, 0, 0);

        _laserCanFire = true;
        _thrusterIsCharging = false;
        _thrusterIsBoosting = false;
        _thrusterCDSlider.value = 0;
        _thrusterUsedTime = 0;
        _thrusterFillDelaySeconds = 0.5f;
        _thrusterCDDelay = 0.2f;
        _speedMultiplier = 1.85f;
        _thrusterFire.gameObject.SetActive(false);
        _currentHealth = 3;

    }

    private void Start()
    {
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
        if (_audioSource == null)
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

        _camShaker = GameObject.Find("Camera_Holder").GetComponent<CameraShake>();
        if (_camShaker == null)
        {
            Debug.LogError("No Cam Holder");
        }
        _camShaker.camShakeActive = true;

        _currentAmmo = maxAmmo;
    }

    void Update()
    {
        CalculateMovement();
        ShootLaser();
        ChargeBlaston();
        ManualThruster();
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
    void ManualThruster()
    {
        //boost
        if (Input.GetKey(KeyCode.LeftShift) && !_thrusterIsCharging)
        {
            if (_thrusterIsBoosting == false)
            {
                _thrusterFire.SetActive(true);
                _speed *= _speedMultiplier;
                _thrusterIsBoosting = true;
            }
            if (_thrusterIsBoosting == true)
            {
                _thrusterUsedTime += Time.deltaTime * _thrusterFillDelaySeconds;
                _thrusterCDSlider.value = _thrusterUsedTime;
                if (_thrusterCDSlider.value >= 1)
                {
                    _thrusterFire.SetActive(false);
                    _speed /= _speedMultiplier;
                    StartCoroutine(ThrusterCoolDown());
                }
            }  
        }
            if (Input.GetKeyUp(KeyCode.LeftShift) && _thrusterIsBoosting)
        {
            _thrusterFire.SetActive(false);
            _speed /= _speedMultiplier;
            _thrusterIsBoosting = false;
        }
        if (_thrusterIsCharging)
        {
            _thrusterCDSlider.value -= Time.deltaTime * _thrusterCDDelay;
        }
    }

   private IEnumerator ThrusterCoolDown()
    {
        _thrusterIsBoosting = false;
        _thrusterIsCharging = true;
        yield return _thrusterRegen;
        _thrusterIsCharging = false;
        _thrusterCDSlider.value = 0;
        _thrusterUsedTime = 0;
    }

    //Start Weapon System
    void ShootLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _blastonIsCharging)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && _laserCanFire && _tripleShotEnabled)
        {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _audioSource.Play();
            _laserCanFire = false;
            StartCoroutine(LaserDelay());
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _laserCanFire && _isAmmoReady)
        {
            if(_currentAmmo == 15)
            {
                _ammoIndicatorRenderer.color = Color.green;
            }
            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = transform.position + new Vector3(0, 1.05f, 0);
                bullet.transform.rotation = Quaternion.identity;
                bullet.SetActive(true);
            }
            _currentAmmo--;
            _audioSource.Play();
            _laserCanFire = false;
            StartCoroutine(LaserDelay());
            if (_currentAmmo == 10)
            {
                _ammoIndicatorRenderer.color = Color.blue;
            }
            else if (_currentAmmo == 5)
            {
                _ammoIndicatorRenderer.color = Color.red;
            }
            else if(_currentAmmo == 0)
            {
                _isAmmoReady = false;
                _ammoIndicator.SetActive(false);

            }
        }
    }
    public void ReloadAmmo()
    {
        _currentAmmo = maxAmmo;
        _ammoIndicatorRenderer.color = Color.green;
        _ammoIndicator.SetActive(true);
        _isAmmoReady = true;
    }
    IEnumerator LaserDelay()
    {
        yield return new WaitForSeconds(_attackDelay);
        _laserCanFire = true;
    }

    void ChargeBlaston()
    {
        if(Input.GetKey(KeyCode.S) && _blastonChargeTime < 2 && !_blastonOnCD)
        {
            _blastonIsCharging = true;
            if(_blastonIsCharging == true)
            {
                _blastonChargeTime += Time.deltaTime * _blastonChargeSpeed;
            }
        }
        if (Input.GetKeyUp(KeyCode.S) && _blastonChargeTime >= 2 && !_blastonOnCD)
        {
            ReleaseBlaston();
        }
        else if (Input.GetKeyUp(KeyCode.S) && _blastonChargeTime < 2)
        {
            _blastonChargeTime = 0;
            _blastonIsCharging = false;
        }
    }
    void ReleaseBlaston()
    {
        Instantiate(_blaston, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        StartCoroutine(BlastonOnCooldown());
    }
    IEnumerator BlastonOnCooldown()
    {
        _blastonIsCharging = false;
        _blastonChargeTime = 0;
        _blastonOnCD = true;
        yield return _blastonRegen;
        _blastonOnCD = false;
    }

    //End of Weapon System
    public void Heal()
    {
        if(_currentHealth != 3)
        {
            _currentHealth++;
            _uiManager.UpdateLives(_currentHealth);
        }
        if(_currentHealth == 2)
        {
            _wFL.SetActive(false);
        }
        else if( _currentHealth == 3) 
        {
            _wFR.SetActive(false);
        }
    }
    public void Damage()
    {
        if (_ShieldEnabled == true)
        {
            _shieldHealth--;

            if (_shieldHealth == 2)
            {
                _shieldRenderer.color = Color.green;
            }
            else if (_shieldHealth == 1)
            {
                _shieldRenderer.color = Color.magenta;
            }
            else if (_shieldHealth == 0)
            {
                _shieldRenderer.color = Color.red;
                StartCoroutine(ShieldTurnOff());
                _ShieldEnabled = false;
            }
            return;
        }
        else if (_ShieldEnabled == false)
        {
            _currentHealth --;
            _uiManager.UpdateLives(_currentHealth);
            

            if (_currentHealth == 2)
            {
                _wFR.SetActive(true);
                _camShaker.traumaMult = 5f;
                _camShaker.traumaDecay = 1.3f;
                _camShaker.trauma = 0.5f;
            }

            if (_currentHealth == 1)
            {
                _wFL.SetActive(true);
                _camShaker.traumaMult = 6.5f;
                _camShaker.traumaDecay = 0.8f;
                _camShaker.trauma = 0.75f;
            }

            if (_currentHealth < 1)
            {
                _camShaker.traumaMult = 8f;
                _camShaker.traumaDecay = 0.5f;
                _camShaker.trauma = 1f;
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


    //PowerUps
    public void ActivateShield()
    {
        _ShieldEnabled = true;
        _shieldGameObj.SetActive(true);
        _shieldHealth = 3;
        _shieldRenderer.color = Color.cyan;
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
            _thrusterFire.SetActive(true);
            _speed *= _speedMultiplier;
            yield return new WaitForSeconds(6.0f);
            _speed /= _speedMultiplier;
            _BoostEnabled = false;
            _thrusterFire.SetActive(false);
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
