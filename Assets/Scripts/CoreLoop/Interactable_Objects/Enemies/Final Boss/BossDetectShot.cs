using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetectShot : MonoBehaviour
{
    private FinalBoss _bossScript;
    // Start is called before the first frame update
    void Start()
    {
        _bossScript= GetComponentInParent<FinalBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _bossScript.TakeDamage();
        }
    }
}
