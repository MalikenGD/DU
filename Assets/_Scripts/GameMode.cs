using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public event Action<GameState> OnGameStateChanged;

    [SerializeField] private GameObject gridManagerPrefab;
    [SerializeField] private GameObject shopPrefab;

    private Transform _parentTransform;
    private GridManager _gridManager;
    private Shop _shop;
    private GameState _currentGameState;
    

    
    private void Start()
    {
        ChangeState(GameState.GameStart);
        

        _gridManager = Instantiate(gridManagerPrefab, transform).GetComponent<GridManager>();
        _shop = Instantiate(shopPrefab, transform).GetComponent<Shop>();
        _shop.SetGridManagerReference(_gridManager);
    }
    private void ChangeState(GameState newGameState)
    {
        
        //TODO: Fill in with logic depending on how to broadcast state across classes
        if (newGameState == _currentGameState) return;
        
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
                Debug.LogError("Game State Out Of Range");
                break;
                
        }
        
        OnGameStateChanged?.Invoke(_currentGameState);
    }
}


public enum GameState
{
    GameStart = 0,
    BuyPhase = 1,
    CombatPhase = 2
}
