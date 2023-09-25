using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
    GameStart = 0,
    BuyPhase = 1,
    CombatPhase = 2
}

public class GameMode : MonoBehaviour
{
    public event Action<GameState> OnGameStateChanged;

    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject shopPrefab;

    private Transform _parentTransform;
    private Grid _grid;
    private Shop _shop;
    private GameState _currentGameState;


    private void Awake()
    {
        GameObject gridObject = Instantiate(gridPrefab, transform);

        if (gridObject is not null)
        {
            _grid = gridObject.GetComponent<Grid>();
        }
        else
        {
            Debug.LogError("GameMode.Start: Grid not valid");
        }
        
        GameObject shopObject = Instantiate(shopPrefab, transform);

        if (shopObject is not null)
        {
            _shop = shopObject.GetComponent<Shop>();
        }
        else
        {
            Debug.LogError("GameMode.Start: Shop not valid");
        }
        
        _shop.SetGridReference(_grid);
    }

    private void Start()
    {
        ChangeState(GameState.GameStart);
    }
    private void ChangeState(GameState newGameState)
    {
        
        //TODO: Fill in with logic depending on how to broadcast state across classes
        if (newGameState == _currentGameState)
        {
            return;
        }
        
        _currentGameState = newGameState;
        switch (_currentGameState)
        {
            case GameState.GameStart:
                
                break;
            case GameState.BuyPhase:
                
                break;
            case GameState.CombatPhase:

                break;
            default:
                Debug.LogError("GameMode.ChangeState: Game State Out Of Range");
                break;
                
        }
        
        OnGameStateChanged?.Invoke(_currentGameState);
    }
}
