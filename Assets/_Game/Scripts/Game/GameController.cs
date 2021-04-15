using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppCore;
using DG.Tweening;

public enum GameState
{
    MainMenu,
    GamePlay,
    Lose,
    Advertising,
    EndAdvertising
}

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UIController _uiController;
    [SerializeField] private Hole _hole;
    [SerializeField] private SoundManager _soundManager;

    public Hole Hole => _hole;
    public SoundManager SoundManager => _soundManager;
    public PlayerController PlayerController => _playerController;
    public UIController UIController => _uiController;
    public int BestScore { get { return DataManager.Instance.DataContainer.BestScore; } set { DataManager.Instance.DataContainer.BestScore = value; } }

    public static GameController Instance { get; private set; }

    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState;

    public int score;
    public float speedMoveHole;
    public float endValue;
    public bool isStart;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ChangeGameState(GameState.MainMenu);

    }

    void Update()
    {
        if(CurrentGameState == GameState.GamePlay)
        {
            if(_playerController.life == 0)
            {
                LoseGame();
            }
            
        }
    }
    public void ChangeGameState(GameState gameState)
    {
        _currentGameState = gameState;
    }

    public void ResetGame()
    {
        ChangeGameState(GameState.GamePlay);
        _hole.transform.position = new Vector2(0, 4);
        _playerController.canPush = false;
        score = 0;
    }

    public void LoseGame()
    {
        _hole.ResetState();
        _playerController.life = 3;
        _soundManager.EffectLose();
        ChangeGameState(GameState.Lose);
        AppCore.AudioController.Instance.StopMusic();
        if(BestScore < score)
            BestScore = score;
        if (BestScore > score)
            return;
    }

    public void ChangeSpeedHole()
    {
        //if (score == 10)
        //{
        //    Hole.MoveHole();
        //}

        if (score >= 10 && score % 5 == 0)
        {
            //float time = 1;
            //_hole.tween.DOTimeScale((endValue - time), 0);
            //_hole.tweenOne.Kill();
            //_hole.tweenSecond.Kill();
            _hole.speed += 1f;
            //_hole.MoveHole();
        }
        if(score % 10 == 0)
        {
            _soundManager.EffectScore();
        }
    }
    
}
