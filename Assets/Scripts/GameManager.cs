using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public enum TurnOptions
{
    Player, Opponent,
}

enum GameStates
{
    PlayerTurn,
    PlayerRollingDice,
    PlayerAfterDiceRolled,
    PlayerMoving,
    PlayerWon,

    OpponentTurn,
    OpponentRollingDice,
    OpponentAfterDiceRolled,
    OpponentMoving,
    OpponentWon,
    SwitchingCamera,
}
public class GameManager : MonoBehaviour
{

    [SerializeField]
    CinemachineBrain _cinemachineBrain;

    [SerializeField]
    Board _board = default;

    [SerializeField]
    Character _player = default, _opponent = default;

    [SerializeField]
    Dice _dice = default;

    [SerializeField, Min(1f)]
    float _delayToSeeTheDice = 1f;

    [SerializeField]
    TurnOptions _startTurn = default;

    TurnOptions _currentTurn = default;

    GameStates _gameState;

    void Awake()
    {
        _board.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

        _dice.onDiceRollResult.AddListener(HandleDiceResult);
        _player.onReachedDestination.AddListener(HandlePlayerReachedDestination);
        _opponent.onReachedDestination.AddListener(HandleOpponentReachedDestination);

        var firstTile = _board.GetTile(1);

        _player.PlaceOn(firstTile.PlayerRoom.position);
        _opponent.PlaceOn(firstTile.OpponentRoom.position);

        _currentTurn = _startTurn;
        _gameState = _currentTurn == TurnOptions.Player ? GameStates.PlayerTurn : GameStates.OpponentTurn;

    }


    private void HandlePlayerReachedDestination(int destinationIndex)
    {
        if (destinationIndex == Board.Instance.MaxIndex)
        {
            _gameState = GameStates.PlayerWon;
            return;
        }

        _gameState = GameStates.SwitchingCamera;
        StartCoroutine(SwitchCamera(() => SwitchToOpponent()
        , () =>
        {
            _gameState = GameStates.OpponentTurn;
        }));
    }

    private void HandleOpponentReachedDestination(int destinationIndex)
    {
        if (destinationIndex == Board.Instance.MaxIndex)
        {
            _gameState = GameStates.OpponentWon;
            return;
        }

        _gameState = GameStates.SwitchingCamera;
        StartCoroutine(SwitchCamera(() => SwitchToPlayer()
        , () =>
        {
            _gameState = GameStates.PlayerTurn;
        }));
    }

    private void HandleDiceResult(int result)
    {
        _gameState = GameStates.SwitchingCamera;

        StartCoroutine(DelayAndContinue(_delayToSeeTheDice, () => MoveTheCharacter(result)));
    }

    private void MoveTheCharacter(int movesCount)
    {
        if (_currentTurn == TurnOptions.Player)
        {
            StartCoroutine(SwitchCamera(() => SwitchToPlayer()
            , () =>
            {
                _player.AddDiceResult(movesCount);
                _gameState = GameStates.PlayerAfterDiceRolled;
            }));
        }
        else
        {
            StartCoroutine(SwitchCamera(() => SwitchToOpponent()
            , () =>
            {
                _opponent.AddDiceResult(movesCount);
                _gameState = GameStates.OpponentAfterDiceRolled;
            }));
        }
    }

    private void RollDice()
    {
        _dice.Roll();
        UI.Instance.HideUI();
    }

    private IEnumerator SwitchCamera(Action switchCameraAction, Action afterSwitchingCameraAction)
    {
        switchCameraAction.Invoke();

        yield return new WaitUntil(() => _cinemachineBrain.IsBlending);

        yield return new WaitUntil(() => !_cinemachineBrain.IsBlending);

        afterSwitchingCameraAction.Invoke();
    }

    private IEnumerator DelayAndContinue(float delay, Action afterDelayAction)
    {
        yield return new WaitForSeconds(delay);

        afterDelayAction.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (UI.Instance == null)
        {
            return;
        }

        switch (_gameState)
        {
            case GameStates.PlayerTurn:
                _dice.PlaceOn(_player.DiceLocation);
                _currentTurn = TurnOptions.Player;
                UI.Instance.ShowMessage("Press Space to Roll the Dice");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RollDice();
                    _gameState = GameStates.PlayerRollingDice;
                }
                break;
            case GameStates.PlayerAfterDiceRolled:
                _player.CreateWayPoints();
                _gameState = GameStates.PlayerMoving;
                break;
            case GameStates.PlayerWon:
                UI.Instance.ShowMessage("Player Won");
                break;


            case GameStates.OpponentTurn:
                _dice.PlaceOn(_opponent.DiceLocation);
                _currentTurn = TurnOptions.Opponent;
                UI.Instance.ShowMessage("Press Space to Roll the Dice");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RollDice();
                    _gameState = GameStates.OpponentRollingDice;
                }
                break;
            case GameStates.OpponentAfterDiceRolled:
                _opponent.CreateWayPoints();
                _gameState = GameStates.OpponentMoving;
                break;
            case GameStates.OpponentWon:
                UI.Instance.ShowMessage("Opponent Won");
                break;
        }
    }

    private void LateUpdate()
    {
        switch (_gameState)
        {
            case GameStates.PlayerTurn:
            case GameStates.PlayerWon:
                SwitchToPlayer();
                break;

            case GameStates.OpponentTurn:
            case GameStates.OpponentWon:
                SwitchToOpponent();
                break;

            case GameStates.PlayerRollingDice:
            case GameStates.OpponentRollingDice:
                SwitchToDice();
                break;
        }
    }

    private void SwitchToDice()
    {
        _dice.Camera.Priority = 2;
        _player.Camera.Priority = 1;
        _opponent.Camera.Priority = 0;
    }

    private void SwitchToOpponent()
    {
        _opponent.Camera.Priority = 2;
        _player.Camera.Priority = 1;
        _dice.Camera.Priority = 0;
    }

    private void SwitchToPlayer()
    {
        _player.Camera.Priority = 2;
        _opponent.Camera.Priority = 1;
        _dice.Camera.Priority = 0;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
