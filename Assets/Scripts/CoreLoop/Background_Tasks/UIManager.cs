using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _destroyedScore;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private TMP_Text _gameOverTxt;
    [SerializeField]
    private TMP_Text _restartTxt;
    [SerializeField]
    private TMP_Text _ammoTxt;
    public Slider thrusterSlider;

    public int shipsDestroyed;

    private bool _isPaused;
    [SerializeField]
    private TMP_Text _pauseTxt;

    void Start()
    {
        shipsDestroyed= 0;
        _destroyedScore.text = "Enemies to kill: 0";
        _gameOverTxt.gameObject.SetActive(false);
        _restartTxt.gameObject.SetActive(false);
        _pauseTxt.gameObject.SetActive(false);
        thrusterSlider.value = 0;
        _isPaused = false;
    }


    public void NewTakedown(int playerScore)
    {
        _destroyedScore.text = "Enemies to kill: " + playerScore.ToString();
    }

    public void AmmoDisplay(int ammoCount, int maxAmmo)
    {
        _ammoTxt.text = "Ammo: " + ammoCount.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives >= 0)
        {
            _livesImg.sprite = _livesSprites[currentLives];
        }
    }

    public void PauseGame()
    {
        if (!_isPaused)
        {
            Time.timeScale = 0.0f;
            _isPaused = true;
            _pauseTxt.gameObject.SetActive(true);
        }
        else if (_isPaused)
        {
            Time.timeScale = 1.0f;
            _isPaused = false;
            _pauseTxt.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        _gameOverTxt.gameObject.SetActive(true);
        _restartTxt.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    IEnumerator GameOverFlicker()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.25f);
            _gameOverTxt.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _gameOverTxt.gameObject.SetActive(true);
        }
    }
}
