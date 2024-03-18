using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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
    [SerializeField] private GameObject playerPrefab;

    private Transform _parentTransform;
    private Player _player;
    private Grid _grid;
    private Shop _shop;
    private GameState _currentGameState;
    [SerializeField] private AIController aiControllerPrefab;


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
        
        GameObject playerGameObject = Instantiate(playerPrefab);
        if (playerGameObject != null)
        {
            _player = playerGameObject.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("World.Awake: Player not valid.");
        }
        
        GameObject shopObject = Instantiate(shopPrefab, transform);

        if (shopObject is not null)
        {
            _shop = shopObject.GetComponent<Shop>();
            _shop.SetGridReference(_grid); // So Shop can access/set selectedCell
            _shop.SetPlayerReference(_player); // For spending currency to buy item
            _shop.OnGridUnitCreated += CreateAIController;
        }
        else
        {
            Debug.LogError("GameMode.Start: Shop not valid");
        }
    }

    private void Start()
    {
        ChangeState(GameState.GameStart);
    }

    private void CreateAIController(Unit unit, UnitCombatDataSO unitCombatDataSO)
    {
        AIController aiController = Instantiate(aiControllerPrefab, transform.parent, true);
        aiController.Initialize(unit, unitCombatDataSO);
        
        aiController.transform.parent = unit.transform;
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
                //Loop through units and resetpos. 
                //Return units to origin
                //Pay round income
                
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
