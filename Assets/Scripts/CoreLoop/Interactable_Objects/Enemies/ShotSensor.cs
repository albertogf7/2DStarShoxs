using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSensor : MonoBehaviour
{
    [SerializeField]
    private bool shotAvoided = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" && !shotAvoided)
        {
            transform.GetComponentInParent<Enemy>().AvoidShot();
            shotAvoided= true;
        }
    }
}
