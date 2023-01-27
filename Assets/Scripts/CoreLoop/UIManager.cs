using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


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

    public int shipsDestroyed;
     
    void Start()
    {
        shipsDestroyed= 0;
        _destroyedScore.text = "Destroyed: " + shipsDestroyed;
        _gameOverTxt.gameObject.SetActive(false);
        _restartTxt.gameObject.SetActive(false);
    }


    public void NewTakedown(int playerScore)
    {
        _destroyedScore.text = "Destroyed: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];
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
