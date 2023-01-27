using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularLaserSound : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;
    [SerializeField]
    private float trackID;
    [SerializeField]
    private AudioClip[] _track1Clips;
    [SerializeField]
    private AudioClip[] _track2Clips;
    [SerializeField]
    private AudioClip[] _track3Clips;
    [SerializeField]
    private AudioClip[] _track4Clips;
    [SerializeField]
    private AudioClip[] _track5Clips;
    [SerializeField]
    private AudioClip[] _track6Clips;

    private Player player;
    [SerializeField]
    private AudioSource _modularSource;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("player missing");
        }
    }

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

            AudioSource.PlayClipAtPoint(_track1Clips[0], transform.position);

            if (player != null)
            {
                switch(trackID) 
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

    public void PlayCurrentSample()
    {
        int randomSample = Random.Range(0, 2);
        switch (trackID)
        {
            case 0:
                _modularSource.clip = _track1Clips[randomSample];
                _modularSource.Play();
                break;
            case 1:
                _modularSource.clip = _track2Clips[randomSample];
                _modularSource.Play();
                break;
            case 2:
                _modularSource.clip = _track3Clips[randomSample];
                _modularSource.Play();
                break;
            case 3:
                _modularSource.clip = _track4Clips[randomSample];
                _modularSource.Play();
                break;
            case 4:
                _modularSource.clip = _track5Clips[randomSample];
                _modularSource.Play();
                break;
            case 5:
                _modularSource.clip = _track6Clips[randomSample];
                _modularSource.Play();
                break;
        }
    }

    public void SetTrackID()
    {
        float setTime = (float)Time.deltaTime;

        if (setTime < 301)
        {
            trackID= 0;
        }
        else if (setTime > 300 && setTime < 601)
        {
            trackID = 1;
        }
        else if (setTime > 600 && setTime < 901)
        {
            trackID = 2;
        }
        else if (setTime > 900 && setTime < 1201)
        {
            trackID = 3;
        }
        else if (setTime > 1200 && setTime < 1501)
        {
            trackID = 4;
        }
        else if (setTime > 1500 && setTime < 1801)
        {
            trackID = 5;
        }



    }
}
