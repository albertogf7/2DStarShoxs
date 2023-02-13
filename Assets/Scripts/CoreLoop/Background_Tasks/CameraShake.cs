using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    #region variables
    public bool camShakeActive;
    [Range(0, 1)] public float trauma;
    private float _timeCounter;
    public float traumaMult = 5f;
    [SerializeField] 
    private float _traumaMag = 0.8f;
    [SerializeField]
    private float _traumaRotMag = 1.7f;
    public float traumaDecay = 1.3f;

    #endregion

    #region accessors
    public float Trauma
    {
        get 
        { 
            return trauma; 
        }
        set
        { 
            trauma = Mathf.Clamp01(value); 
        }
    }

    #endregion

    #region methods
    float GetFloat(float seed)
    {
        return (Mathf.PerlinNoise(seed, _timeCounter) - 0.5f) * 2;
    }

    Vector3 GetVec3()
    {
        return new Vector3(
            GetFloat(1),
            GetFloat(10),
            0
            );
    }

    void CamShake()
    {
        if (camShakeActive && trauma > 0)
        {
            _timeCounter += Time.deltaTime * Mathf.Pow(Trauma, 0.3f) * traumaMult;
            Vector3 newPos = GetVec3() * _traumaMag * trauma;
            transform.localPosition = newPos;
            transform.localRotation = Quaternion.Euler(newPos * _traumaRotMag);
            trauma -= Time.deltaTime * traumaDecay * Trauma;
        }
        else
        {
            Vector3 newPos = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
            transform.localPosition = newPos;
            transform.localRotation = Quaternion.Euler(newPos * _traumaRotMag);
        }
    }

    private void Update()
    {
        CamShake();
    }
    #endregion
}
