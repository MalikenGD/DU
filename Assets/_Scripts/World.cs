using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Serialization;

public class World : MonoBehaviour
{
    public static World Instance;
    [SerializeField] private Canvas parentCanvasPrefab;
    private Canvas _parentCanvas;

    public event Action<GameState> OnGameStateChanged;

    //TODO: Should I use a dictionary for only two lists? Like Dict<Faction, List<Unit>>? 
    private List<Unit> enemyUnits;
    private List<Unit> playerUnits;
    
    //TODO: Does World or GameMode need data?
    [SerializeField] private GameObject gameModePrefab;
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
        
        _parentCanvas = Instantiate(parentCanvasPrefab);
        _uiManager = new UIManager();
        _gameMode = Instantiate(gameModePrefab, transform).GetComponent<GameMode>();
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
    

    public Canvas GetParentCanvas()
    {
        return _parentCanvas;
    }

    public UIBehaviour BuildUI(GameObject uiPrefab, object builder)
    {
        return _uiManager.BuildUI(uiPrefab, builder);
    }
}

