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

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

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
                }
            }
            Destroy(this.gameObject);
        }
    }
}
