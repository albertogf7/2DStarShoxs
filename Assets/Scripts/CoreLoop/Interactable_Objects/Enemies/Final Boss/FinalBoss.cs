using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    #region Handlers
    [SerializeField]
    private int _health;

    private bool _isBossDead;

    private float _glidingSpeed;
    private float _rampageSpeed;
  //  private Player _playerScript;

    [SerializeField]
    private int _attackID; //switch

    [SerializeField]
    private GameObject _enemyLasers;
    [SerializeField]
    private GameObject _sweeperLeft;
    [SerializeField]
    private GameObject _sweeperLaserLeft;
    [SerializeField]
    private GameObject _sweeperRight;
    [SerializeField]
    private GameObject _sweeperLaserRight;
    [SerializeField]
    private GameObject _slowLaser;
    [SerializeField]
    private SpriteRenderer _bossShield;
    [SerializeField]
    private Color32 _shieldColor; //88FF4C
    [SerializeField]
    private SpriteRenderer _bossRenderer;
    [SerializeField]
    private Color _bossColor; //CDFFB8

    [SerializeField]
    private GameObject[] _allBalls;

    [SerializeField]
    private bool _ballsDown;
    private bool _glidingLeft;
    private float _fireRate;
    private float _canFire = -1;

    private bool _canMove;
    private bool _canReposition;
    private float _t;
    [SerializeField]
    private bool _glidingUp;

    private GameManager _gameManager;

    #endregion
    #region Awake n Update
    void Start()
    {
        #region Calling
        _bossRenderer = GetComponentInChildren<SpriteRenderer>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        #endregion
        #region start Handlers
        _health = 3;
        _isBossDead = false;
        _canMove = false;
        _glidingLeft = true;
        _glidingUp = true;
        _canReposition = true;

        _glidingSpeed = 0.55f;
        _rampageSpeed = 1.5f;
        _bossRenderer.color = _bossColor;
        _bossColor = new Color(205, 255, 184, 0);
        StartCoroutine(FadeTo(1.0f, 1.0f));
        _bossShield.gameObject.SetActive(true);
        _sweeperLaserLeft.gameObject.SetActive(false);
        _sweeperLaserRight.gameObject.SetActive(false);


        StartCoroutine(Rampage());

        #endregion
    }
    void BossRendererUpdate()
    {
        _bossRenderer.color = _bossColor;
    }


    void Update()
    {
        
        BossRendererUpdate();
        CheckBalls();   
        if (_canMove)
        {
            SetAttackID();
        }
        //Use Coroutines to animate attacks, use update only for movements.
        //

    }
    #endregion
    #region HealthManager
    public void TakeDamage()
    {
        if (_ballsDown == false)
        {
            Debug.Log("JAJA");
            return;
        }
        else if (_ballsDown)
        {
            _ballsDown = false;
            ResetBallsBools();
            Debug.Log("ouch");
            _health--;
            _canMove = false;
            StartCoroutine(Rampage());

        }
    }
    #region Death Circles
  
    void CheckBalls()
    {
        if (_ballsDown)
        {
            return;
        }
        if (_ballsDown == false)
        {
            if (!_allBalls[0].activeInHierarchy && !_allBalls[1].activeInHierarchy && !_allBalls[2].activeInHierarchy)
            {
                Debug.Log("balls & shield down");
                _ballsDown = true;
                _bossShield.gameObject.SetActive(false);
            }
        }
    }

    void ResetBallsBools()
    {
        foreach (GameObject ball in _allBalls)
        {
            ball.gameObject.SetActive(true);
        }
    }
    void OnBossDeath()
    {
        _isBossDead = true;
        this.gameObject.SetActive(false);
        if (_isBossDead)
        {
            _gameManager.BossDefeated();
            Debug.Log("Boss Defeated");
            _isBossDead = false;
        }
    }
    #endregion

    #endregion

    #region AttacksMethods
    void SetAttackID()
    {
        switch (_attackID)
        {
            case 0:
                RampageAttack();
                break;
            case 1:
                BossMovement();
                GlidingAttack();
                //attack
                break;
            case 2:
                BossMovement();
                SweepingAttack();
                //sweep
                break;
            case 3:
                BossMovement();
                FreezingAttack();
                //freezemovement
                break;
            case 4:
                OnBossDeath();
                break;
        }
    }

    void RampageAttack()
    {
        float x = 0;
        float y = -7.5f;
        float z = 0;
        Vector3 downPos = new Vector3(x, y, z);
        transform.position = Vector3.Lerp(this.transform.position, downPos, _rampageSpeed * Time.deltaTime);

        if (transform.position.y <= -7 && _canReposition)
        {
            StartCoroutine(RepositioningToAttack());
        }
        
    }

    IEnumerator Rampage()
    {
        yield return new WaitForSeconds(1.5f);
        _attackID = 0;
        _canMove = true;
        yield return new WaitForSeconds(4);
        if (_health == 3)
        {
            _attackID = 1;
        }
        else if (_health == 2)
        {
            _attackID = 2;
        }
        else if (_health == 1)
        {
            _attackID = 3;
        }
        else if (_health <= 0)
        {
            _attackID= 4;
        }
        
    }


    void BossMovement()
    {
        switch (_attackID)
        {
            //gliding parameters
            case 1:
                _glidingSpeed = 2f;
                _t = 3f;
                Glide();
                break;
            case 2:
                _glidingSpeed = 1f;
                _t = 3f;
                Glide();
                break;
            case 3:
                _glidingSpeed = 1f;
                _t = 4f;
                Glide();
                break;
        }
        
    }
    void Glide()
    {
        if (_glidingLeft)
        {
           transform.Translate(Vector3.left * (Mathf.SmoothStep(_glidingSpeed, _glidingSpeed * 2, _t) * Time.deltaTime), Space.World);

            if (transform.position.x <= -7.5f)
            {
                _glidingLeft = false;
            }
        }
        else
        {
          transform.Translate(Vector3.right * (Mathf.SmoothStep(_glidingSpeed, _glidingSpeed * 2, _t) * Time.deltaTime), Space.World);

            if (transform.position.x >= 7.5f)
            {
                _glidingLeft = true;
            }
        }
        if (_attackID == 2 || _attackID == 3)
        {
            
            if (_glidingUp)
            {
                transform.Translate(Vector3.up * (Mathf.SmoothStep(_glidingSpeed, _glidingSpeed * 2, _t) * Time.deltaTime), Space.World);
                if (transform.position.y >= 6.5f)
                {
                    _glidingUp = false;
                }
            }
            else
            {
                transform.Translate(Vector3.down * (Mathf.SmoothStep(_glidingSpeed, _glidingSpeed * 2, _t) * Time.deltaTime), Space.World);
                if (transform.position.y <= 2.5f)
                {
                    _glidingUp = true;
                }
            }
            
        }
    }
    void GlidingAttack()
    {
        if ((_allBalls[0].activeInHierarchy))
        {
            if (Time.time > _canFire)
            {
                _fireRate = Random.Range(1.5f, 4.5f);
                _canFire = Time.time + _fireRate;
                GameObject enemyLaser = Instantiate(_enemyLasers, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignEnemyLaser();
                }
            }
        }
    }

    void SweepingAttack()
    {

        if (_glidingLeft)
        {
            _sweeperLeft.transform.Rotate(0f, 0f, 0.15f);
            _sweeperRight.transform.rotation = Quaternion.Euler(0, 0, 90);
            _sweeperLaserLeft.gameObject.SetActive(true);
            _sweeperLaserRight.gameObject.SetActive(true);
                                               //Quaternion.Slerp(this.transform.rotation, , (0.3f * Time.deltaTime));
        }
        else
        {
            _sweeperRight.transform.Rotate(0f, 0f, -0.15f);
            _sweeperLeft.transform.rotation = Quaternion.Euler(0, 0, -90);
            _sweeperLaserLeft.gameObject.SetActive(true);
            _sweeperLaserRight.gameObject.SetActive(true);
        }
        if (_allBalls[0].activeInHierarchy)
        {
            if (Time.time > _canFire)
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
        
    }

    void FreezingAttack()
    {
        //position at center, shoot slowlasers, move to player position, sweep lasers.
        
        if (_glidingLeft)
        { 
            _sweeperLaserLeft.gameObject.SetActive(true);
            _sweeperLaserRight.gameObject.SetActive(true);
            _sweeperRight.transform.rotation = Quaternion.Euler(0, 0, 90);
            _sweeperLeft.transform.Rotate(0f, 0f, 0.25f);
        }
        else
        {
            _sweeperLaserLeft.gameObject.SetActive(true);
            _sweeperLaserRight.gameObject.SetActive(true);
            _sweeperRight.transform.Rotate(0f, 0f, -0.25f);
            _sweeperLeft.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (_allBalls[0].activeInHierarchy)
        {
            if (Time.time > _canFire)
            {
                _fireRate = Random.Range(2, 4);
                _canFire = Time.time + _fireRate;
                GameObject slowLaser = Instantiate(_slowLaser, transform.position, Quaternion.identity);
            }
        }
    }

    #endregion

    #region AnimationCoroutines
    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = _bossColor.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            _bossColor.a = Mathf.Lerp(alpha, aValue, t);
            yield return null;
        }
    }

    IEnumerator RepositioningToAttack()
    {
        _canReposition = false;
        StartCoroutine(FadeTo(0.0f, 1.0f));
        yield return new WaitForSeconds(1);
        float randomX = Random.Range(-8f, 8f);
        transform.position = new Vector3(randomX, 4.5f, 0);
        _canMove = false;
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeTo(1.0f, 1.0f));
        _bossShield.gameObject.SetActive(true);
        ResetBallsBools();
        _canMove = true;
        _canReposition = true;
    }
    #endregion  

}
