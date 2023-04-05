using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
           this.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Homing Missile")
        {
            this.gameObject.SetActive(false);
            HomingMissile hmScript = other.GetComponent<HomingMissile>();
            hmScript.OriginalPosition();
        }
    }
}
