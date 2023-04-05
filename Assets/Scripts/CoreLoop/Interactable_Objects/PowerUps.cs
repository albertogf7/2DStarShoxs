using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;
    [SerializeField]
    private int powerUpID;
    [SerializeField]
    private AudioClip _powerUpClip;
    [SerializeField]
    private AudioClip _explosionClip;

    private Transform _playerTransform;
    [SerializeField]
    private bool _magnetFound = false;

    private void Start()
    {
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }
    void Update()
    {
        if (!_magnetFound)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if (_magnetFound) 
        {
            MoveTowardsPlayer();
        }

        if (transform.position.y < -6.5)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerUpClip, transform.position);

            if (player != null)
            {
                switch(powerUpID) 
                {
                    case 0:
                        player.ActivateTripleShot();
                        break;
                    case 1:
                        player.ActivateBoost(); 
                        break;
                    case 2:
                        player.ActivateShield();
                        break;
                    case 3:
                        player.SlowDown();
                        break;
                    case 4:
                        player.ActivateHomingMissile();
                        break;
                }
            }
            Destroy(this.gameObject);
        }
        else if (other.tag == "Enemy Laser")
        {
            AudioSource.PlayClipAtPoint(_explosionClip, transform.position);
            Destroy(this.gameObject);
        }
        else if (other.tag == "PlayerMagnet")
        {
            _magnetFound = true;
        }
        else
        {
            _magnetFound = false;
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector3.Lerp(this.transform.position, _playerTransform.transform.position, 1.5f * Time.deltaTime);
    }
}
