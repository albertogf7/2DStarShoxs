using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private bool _isSeeking;
    [SerializeField]
    private GameObject _playerShip;
    private GameObject _enemyTarget;
    private Rigidbody2D _rigidBody;
    private float _rotateSpeed = 200;
    private float _speed = 5f;
    private Transform _originalPosition;

   // private bool _onboardingMessage;
    [SerializeField]
    private GameObject _missileThruster;
    [SerializeField]
    private bool _targetFound;
    [SerializeField]
    private SpriteRenderer _targetIndicator;

    // Start is called before the first frame update
    void Start()
    {
       // _onboardingMessage= true;

        _targetFound = false;
        _rigidBody = GetComponent<Rigidbody2D>();
        _isSeeking = false;
        OriginalPosition();
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_targetFound && _isSeeking)
        {
            _enemyTarget = GameObject.FindGameObjectWithTag("Sweeper");
            if (_enemyTarget == null)
            {
                Debug.Log("enemy not yet found");
            }
            else if (_enemyTarget != null) 
            { 
                _targetFound = true;
                _targetIndicator.color = Color.green;
            }
        }
        if (_isSeeking)
        {
            SeekTarget();
        }
    }
    public void ActivateSeeking()
    {
        _isSeeking = true;
        
    }

    void SeekTarget()
    {
        _missileThruster.SetActive(true);
        this.transform.SetParent(null);
        if (!_targetFound)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (transform.position.y > 8)
            {
                OriginalPosition();
            }
        }
        else if (_targetFound)
        {
            if (_enemyTarget == null)
            {
                _targetFound = false;
                _isSeeking = false;
                OriginalPosition();
            }
            else
            {
                Vector2 direction = (Vector2)_enemyTarget.transform.position - _rigidBody.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.up).z;

                _rigidBody.angularVelocity = -rotateAmount * _rotateSpeed;
                _rigidBody.velocity = transform.up * _speed;
            }   
        }
    }

    public void OriginalPosition()
    {
        _missileThruster.gameObject.SetActive(false);
        this.transform.SetParent(_playerShip.transform);
        transform.position = _playerShip.transform.position;
        _targetIndicator.color = Color.red;
        _targetFound= false;
        _enemyTarget = null;
        _isSeeking= false;
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy Laser")
        {
            OriginalPosition();
        }
    }
}
