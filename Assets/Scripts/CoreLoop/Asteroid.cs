using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 19.0f;

    [SerializeField]
    private GameObject _explosionFire;
    private SpriteRenderer _asteroidSprite;

    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
       transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        _asteroidSprite = GetComponent<SpriteRenderer>();
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                Instantiate(_explosionFire, transform.position, Quaternion.identity);
                player.Damage();
                Destroy(this.gameObject, 0.45f);
            }
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (other != null)
            {
                Destroy(other.gameObject);
                _spawnManager.StartSpawning();
                Instantiate(_explosionFire, transform.position, Quaternion.identity);
                Destroy(this.gameObject, 0.45f);
            }
        }
    }

}
