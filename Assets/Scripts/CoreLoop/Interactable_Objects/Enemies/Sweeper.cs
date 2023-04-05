using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweeper : MonoBehaviour
{
    [SerializeField]
    private GameObject _sweepingLaser;
    [SerializeField]
    private int _sweeperID;
    [SerializeField]
    private int _sweeperHealth;
    [SerializeField]
    private bool _isSweeping;
    private Quaternion _originalRotation;

    [SerializeField]
    private Transform _posRight;
    [SerializeField]
    private Transform _posLeft;
    [SerializeField]
    private bool _canMove;
    [SerializeField]
    private bool _canGoDown;

    private SpawnManager _spawnManagerScript;

    [SerializeField]
    private GameObject _explosionFire;
    [SerializeField]
    private AudioClip _explosionClip;

    #region Start, Update
    void Start()
    {
        _spawnManagerScript = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManagerScript == null)
        {
            Debug.Log("SpawnManager is null");
        }
        this.transform.rotation = Quaternion.Euler(0, 0, 45);
        _sweeperID = Random.Range(0, 2);
        _sweeperHealth = 2;
        _isSweeping = false;
        _canMove = false;
        _canGoDown = false;
        _sweepingLaser.gameObject.SetActive(false);
        _originalRotation = this.transform.rotation;
        StartCoroutine(PositioningDelay());
    }

    void Update()
    {
        if (_canMove)
        {
            GetInPoisition();
        }
        if (_isSweeping && _sweeperID == 0)
        {
            SwipeLeft();
        }
        else if (_isSweeping && _sweeperID == 1)
        {
            SwipeRight();
        }
        if (_canGoDown)
        {
            MoveDown();
        }
    }

    #region Movement Attack Cycle
    void GetInPoisition()
    {
        if (!_isSweeping)
        {
            switch (_sweeperID)
            {
                case 0:
                    transform.position = Vector3.Lerp(this.transform.position, _posRight.transform.position, 1.5f * Time.deltaTime);
                    StartCoroutine(ChargeAttack());
                    break;
                case 1:
                    transform.position = Vector3.Lerp(this.transform.position, _posLeft.transform.position, 1.5f * Time.deltaTime);
                    StartCoroutine(ChargeAttack());
                    break;
            }
        }
    }
    IEnumerator PositioningDelay()
    {
        yield return new WaitForSeconds(2f);
        _canMove = true;
    }
    IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(1.8f);
        _sweepingLaser.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _isSweeping = true;
        _canMove = false;
        StartCoroutine(WaitForSweep());
    }
    void SwipeLeft()
    {
        transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, 0, -45), (0.3f * Time.deltaTime));
    }
    void SwipeRight()
    {
        transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, 0, 135), (0.3f * Time.deltaTime));
    }
    #endregion

    void MoveDown()
    {
        // transform.Translate(Vector3.down * 2 * Time.deltaTime);
        transform.Translate(Vector3.down * (2 * Time.deltaTime), Space.World);
        if (transform.position.y < -6)
        {
            _canGoDown = false;
            _isSweeping = false;
            _sweepingLaser.gameObject.SetActive(false);
            _canMove = true;
            this.transform.rotation = _originalRotation;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            other.gameObject.SetActive(false);
            SweeperDamaged();
        }
        if (other.tag == "Homing Missile")
        {
            _sweeperHealth = 1;
            HomingMissile hmScript = other.GetComponent<HomingMissile>();
            hmScript.OriginalPosition();
            SweeperDamaged();
        }
    }

   void SweeperDamaged()
    {
        _sweeperHealth -= 1;
        if (_sweeperHealth < 1)
        {
            _spawnManagerScript.EnemyDestroyed(1);
            Instantiate(_explosionFire, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_explosionClip, transform.position);
            Destroy(this.gameObject);
        }
    }

    IEnumerator WaitForSweep()
    {
        yield return new WaitForSeconds(10);
        _canGoDown = true;
    }
    #endregion
}
