using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBeam : MonoBehaviour
{
    private bool _isSeeking;
    private Player _player;
    private Transform _target;
    private Rigidbody2D _rigidBody;
    private float _rotateSpeed = 200;
    private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player is null");
        }
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _rigidBody = GetComponent<Rigidbody2D>();
        _isSeeking = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        if (_target != null)
        {
            Vector2 direction = (Vector2)_target.position - _rigidBody.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            _rigidBody.angularVelocity = -rotateAmount * _rotateSpeed;
            _rigidBody.velocity = transform.up * speed;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player.Damage();
            Destroy(this.gameObject);
        }
        else if (other.tag == "Laser")
        {
            Destroy(this.gameObject);
        }
    }
}
