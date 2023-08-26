using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode
{
    public event Action<GameState> OnGameStateChanged;



    private GridManager _gridManager;
    private Shop _shop;
    private GameState currentGameState;
    

    
    public GameMode()
    {
        ChangeState(GameState.GameStart);
        _gridManager = new GridManager();
        _shop = new Shop(Resources.Load<UnitCompleteListSO>("Unit/UnitCompleteList").GetList());
    }
    private void ChangeState(GameState newGameState)
    {
        
        //TODO: Fill in with logic depending on how to broadcast state across classes
        if (newGameState == currentGameState) return;
        
        currentGameState = newGameState;
        switch (currentGameState)
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
        
        OnGameStateChanged?.Invoke(currentGameState);
    }
    
}


public enum GameState
{
    GameStart = 0,
    BuyPhase = 1,
    CombatPhase = 2
}
