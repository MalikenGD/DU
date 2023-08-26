using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    public event Action<GameState> OnGameStateChanged;

    //TODO: Should I use a dictionary for only two lists? Like Dict<Faction, List<Unit>>? 
    private List<Unit> enemyUnits;
    private List<Unit> playerUnits;
    
    //TODO: Does World or GameMode need data?
    private GameMode _gameMode;
    private UIManager _uiManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _uiManager = Instantiate(Resources.Load<UIManager>("UI/UIManager").GetComponent<UIManager>());
        _gameMode = new GameMode();
    }

    private void OnEnable()
    {
        _gameMode.OnGameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        _gameMode.OnGameStateChanged -= GameStateChanged;
    }


    private void GameStateChanged(GameState currentGameState)
    {
        //Is this best? And have every other script subscribe to this?
        OnGameStateChanged?.Invoke(currentGameState);
    }

    public void CreateUIObject(int selection, object builder)
    {
        //Does it make sense for the World to manage _uiManager and use the UIObjectFactory?
        _uiManager.BuildUI(selection,builder);
    }
}
