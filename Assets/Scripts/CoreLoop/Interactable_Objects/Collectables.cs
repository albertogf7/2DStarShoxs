using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;
    [SerializeField]
    private int collectibleID;
    [SerializeField]
    private AudioClip _powerUpClip;

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
        else if(_magnetFound)
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
                switch(collectibleID) 
                {
                    case 0:
                        player.ReloadAmmo();
                        break;
                    case 1:
                        player.Heal(); 
                        break;
                }
            }
            Destroy(this.gameObject);
        }
        else if (other.tag == "PlayerMagnet")
        {
            _magnetFound = true;
        }
    }
    void MoveTowardsPlayer()
    {
        transform.position = Vector3.Lerp(this.transform.position, _playerTransform.transform.position, 2f * Time.deltaTime);
    }
}
