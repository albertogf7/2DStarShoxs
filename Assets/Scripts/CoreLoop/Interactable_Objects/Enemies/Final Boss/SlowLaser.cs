using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowLaser : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<Player>().SlowDown();
            }
        }
    }
}
