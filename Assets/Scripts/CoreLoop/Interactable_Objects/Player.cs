using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region variable handlers
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
    private GameObject _homingMissile;
    private bool _homingIsReady = false;
    private HomingMissile _homingScript;

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
    private float _thrusterFillDelaySeconds = 0.5f;
    private float _thrusterRegen = 5.0f;
    [SerializeField]
    private float _thrusterUsedTime;
    [SerializeField]
    private bool _thrusterIsCharging;
    [SerializeField]
    private bool _thrusterIsBoosting;
    private float _thrusterCDfloat;
    [SerializeField]
    private GameObject _thrusterFire;
    [SerializeField]
    private GameObject _wingFlareR;
    [SerializeField]
    private GameObject _wingFlareL;
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

    private int _maxAmmo = 20;
    [SerializeField]
    private int _currentAmmo;
    private bool _isAmmoReady = true;
    [SerializeField]
    private GameObject _ammoIndicator;
    [SerializeField]
    private SpriteRenderer _ammoIndicatorRenderer;

    [SerializeField]
    private CameraShake _camShaker;

    [SerializeField]
    private GameObject _magneticCollider;
    private bool _magnetOnCD = false;

    #endregion

    #region Awake, Start, Update
    void Awake()
    {
        transform.position = new Vector3(0, 0, 0);

        _laserCanFire = true;
        _thrusterIsCharging = false;
        _thrusterIsBoosting = false;
        _thrusterUsedTime = 0;
        _thrusterFillDelaySeconds = 0.5f;
        _speedMultiplier = 1.85f;
        _thrusterFire.gameObject.SetActive(false);
        _currentHealth = 3;
        _magneticCollider.gameObject.SetActive(false);
    }
    
    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvass").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("UI is null");
        }
        _wingFlareL.SetActive(false);
        _wingFlareR.SetActive(false);

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
        
        _homingScript = GetComponentInChildren<HomingMissile>();
        if (_homingScript == null)
        {
            Debug.LogError("No homing script");
        }

        _homingMissile.gameObject.SetActive(false);

        _camShaker.camShakeActive = true;

        _currentAmmo = _maxAmmo;
        _uiManager.AmmoDisplay(_currentAmmo, _maxAmmo);
    }

    void Update()
    {
        CalculateMovement();
        ShootLaser();
        ChargeBlaston();
        ManualThruster();
        MagneticFieldOn();
        CheckPP();
    }
    #endregion

    #region Movement
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
        if (Input.GetKey(KeyCode.LeftShift) && !_thrusterIsCharging)
        {
            if (_thrusterIsBoosting == false)
            {
                _thrusterFire.SetActive(true);
                _speed *= _speedMultiplier;
                _thrusterIsBoosting = true;
            }
            else
            {
                _thrusterUsedTime += Time.deltaTime * _thrusterFillDelaySeconds;
                _uiManager.thrusterSlider.value = _thrusterUsedTime;
                if (_thrusterUsedTime >= 1)          
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
    }

    private IEnumerator ThrusterCoolDown()
    {
        _thrusterIsBoosting = false;
        _thrusterIsCharging = true;
        _thrusterCDfloat = 5f;

        while (_thrusterCDfloat >= 0)
        {
            _thrusterCDfloat -= Time.deltaTime;
            _uiManager.thrusterSlider.value = _thrusterCDfloat / _thrusterRegen;
            yield return new WaitForEndOfFrame();
        }
        _thrusterIsCharging = false;
        _uiManager.thrusterSlider.value = 0;
        _thrusterUsedTime = 0;
    }

    private void CheckPP()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _uiManager.PauseGame();
        }
    }
    #endregion

    #region Weapon System
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
            if (_currentAmmo == 15)
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
            _uiManager.AmmoDisplay(_currentAmmo, _maxAmmo);
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
            else if (_currentAmmo == 0)
            {
                _isAmmoReady = false;
                _ammoIndicator.SetActive(false);

            }
        }

        if (Input.GetKeyDown(KeyCode.E) && _homingIsReady)
        {
            _homingScript.ActivateSeeking();
            _homingIsReady = false;
            Debug.Log("homing shot");
        }
    }
    public void AddMaxAmmo()
    {
        _maxAmmo += 7;
        _uiManager.AmmoDisplay(_currentAmmo, _maxAmmo);
    }
    public void ReloadAmmo()
    {
        _currentAmmo = _maxAmmo;
        _ammoIndicatorRenderer.color = Color.green;
        _ammoIndicator.SetActive(true);
        _isAmmoReady = true;
        _uiManager.AmmoDisplay(_currentAmmo, _maxAmmo);
    }
    IEnumerator LaserDelay()
    {
        yield return new WaitForSeconds(_attackDelay);
        _laserCanFire = true;
    }

    void ChargeBlaston()
    {
        if (Input.GetKey(KeyCode.S) && _blastonChargeTime < 2 && !_blastonOnCD)
        {
            _blastonIsCharging = true;
            if (_blastonIsCharging == true)
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
    #endregion

    #region HealthManager
    public void Heal()
    {
        if (_currentHealth != 3)
        {
            _currentHealth++;
            _uiManager.UpdateLives(_currentHealth);
        }
        if (_currentHealth == 2)
        {
            _wingFlareL.SetActive(false);
        }
        else if (_currentHealth == 3)
        {
            _wingFlareR.SetActive(false);
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
            _currentHealth--;
            if (_currentHealth >= 0)
            {
                _uiManager.UpdateLives(_currentHealth);
            }

            if (_currentHealth == 2)
            {
                _wingFlareR.SetActive(true);
                _camShaker.ShakeSetUp(5f, 1.3f, 0.5f);
            }

            if (_currentHealth == 1)
            {
                _wingFlareL.SetActive(true);
                _camShaker.ShakeSetUp(6.5f, 0.8f, 0.75f);
            }

            if (_currentHealth == 0)
            {
                _camShaker.ShakeSetUp(8f, 0.5f, 1f);
                _spawnManager.OnPlayerDeath();
                _gameManager.GameOver();
                _uiManager.GameOver();
                Instantiate(_explosionFire, transform.position, Quaternion.identity);
                _audioSource.clip = _explosionClip;
                _audioSource.pitch = 0.8f;
                _audioSource.volume = 1f;
                _audioSource.Play();
                this.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region PowerUps
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
    
    public void SlowDown()
    {
        StartCoroutine(SlowedDown());
    }
    IEnumerator SlowedDown()
    {
        _speed /= 10;
        yield return new WaitForSeconds(6.0f);
        _speed = 7.5f;
    }

    void MagneticFieldOn()
    {
        if (Input.GetKey(KeyCode.C) && !_magnetOnCD)
        {
            _magneticCollider.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.C) && !_magnetOnCD)
        {
            _magnetOnCD = true;
            StartCoroutine(MagneticFieldDelay());
        }
    }
    IEnumerator MagneticFieldDelay()
    {
        yield return new WaitForSeconds(1.0f);
        _magneticCollider.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        _magneticCollider.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        _magneticCollider.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        _magneticCollider.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _magneticCollider.SetActive(false);
        _magnetOnCD = false;
    }

    public void ActivateHomingMissile()
    {
        _homingMissile.transform.SetParent(this.transform);
        _homingMissile.transform.rotation = Quaternion.identity;
        _homingMissile.gameObject.SetActive(true);
        _homingIsReady = true;
    }
    #endregion
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
