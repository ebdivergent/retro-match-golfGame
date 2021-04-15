using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _losePanel;

    [SerializeField] private Image[] _hearts;

    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;

    public Text score;
    public Text scoreAfterMatch;
    public Text bestScore;
    public Text bestScoreGameOver;

    
    void LateUpdate()
    {
        HealthBar();

        // !!!!REWRITE THIS!!!! //

        HandleScore();
        HandleScoreAfterMatch();
        BestScore();
        BestScoreAfterGame();

        if(GameController.Instance.CurrentGameState == GameState.Lose)
        {
            Lose();
        }
    }

    void HandleScore()
    {
        score.text = GameController.Instance.score.ToString();
    }
    void HandleScoreAfterMatch()
    {
        scoreAfterMatch.text = "Score: " + GameController.Instance.score.ToString();
    }

    public void MainMenu()
    {
        GameController.Instance.ChangeGameState(GameState.MainMenu);
    }

    public void Restart()
    {
        GameController.Instance.ResetGame();
    }
    public void StartGame()
    {
         GameController.Instance.ChangeGameState(GameState.GamePlay);
    }

    public void BestScore()
    {
        bestScore.text = "Best Score: " + GameController.Instance.BestScore.ToString();
    }
    public void BestScoreAfterGame()
    {
        bestScoreGameOver.text = "Best Score: " + GameController.Instance.BestScore.ToString();
    }

    void HealthBar()
    {
        for (int i =0; i < _hearts.Length; i++)
        {
            if(i < GameController.Instance.PlayerController.life)
            {
                _hearts[i].sprite = _fullHeart;
            } 
            else
            {
                _hearts[i].sprite = _emptyHeart;
            }

            if(i < GameController.Instance.PlayerController.numOflife)
            {
                _hearts[i].enabled = true;
            }
            else
            {
                _hearts[i].enabled = false;
            }
        }
    }
    public void Lose()
    {
         _losePanel.SetActive(true);
        GameController.Instance.ChangeGameState(GameState.MainMenu);
    }
    
}
