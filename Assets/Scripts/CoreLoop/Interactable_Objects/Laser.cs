using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 9.5f;
    [SerializeField]
    private bool _isEnemyLaser = false;

    void Update()
    {
       if (_isEnemyLaser == false)
        {
            MoveUp();
        }
       else 
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (transform.position.y > 8)
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.SetActive(false);
            }
           gameObject.SetActive(false);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if (transform.position.y < -8)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null )
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}
