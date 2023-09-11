using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    GameStart = 0,
    BuyPhase = 1,
    CombatPhase = 2
}

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

        GameObject gridManagerObject = Instantiate(gridManagerPrefab, transform);
        if (gridManagerObject != null)
        {
            _gridManager = gridManagerObject.GetComponent<GridManager>();
        }
        else
        {
            Debug.LogError("GameMode.Start: grid manager not valid.");
        }

        GameObject shopObject = Instantiate(shopPrefab, transform);
        if (shopObject != null)
        {
            _shop = shopObject.GetComponent<Shop>();
            _shop.SetGridManagerReference(_gridManager);
        }
        else
        {
            Debug.LogError("GameMode.Start: shop not valid.");
        }
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
                Debug.LogError("GameMode.ChangeState: game state out of range.");
                break;
        }

        OnGameStateChanged?.Invoke(_currentGameState);
    }
}
