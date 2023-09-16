using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class World : MonoBehaviour
{
    public static World Instance;
    [SerializeField] private Canvas screenSpaceParentCanvasPrefab;
    [SerializeField] private Canvas worldSpaceParentCanvasPrefab;
    private Canvas _screenSpaceParentCanvas;
    private Canvas _worldSpaceParentCanvas;

    public event Action<GameState> OnGameStateChanged;

    //TODO: Should I use a dictionary for only two lists? Like Dict<Faction, List<Unit>>? 
    private List<Unit> _enemyUnits = new List<Unit>();
    private List<Unit> _playerUnits = new List<Unit>();
    
    //TODO: Does World or GameMode need data?
    [SerializeField] private GameObject gameModePrefab;
    private GameMode _gameMode;
    private UIManager _uiManager;

    public UIManager UIManager
    {
        get
        {
            return _uiManager;
        }
        private set
        {
            _uiManager = value;
        }
    }

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
        
        _screenSpaceParentCanvas = Instantiate(screenSpaceParentCanvasPrefab);
        _worldSpaceParentCanvas = Instantiate(worldSpaceParentCanvasPrefab);
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

    public void ToggleUIObjectsOfType(GameObject prefab) => _uiManager.ToggleUIObjectsOfType(prefab);

    private void GameStateChanged(GameState currentGameState)
    {
        //Is this best? And have every other script subscribe to this?
        OnGameStateChanged?.Invoke(currentGameState);
    }

    private void Update()
    {
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Debug.Log("toggling");
            _uiManager.ToggleAllUIObjects();
        }
    }

    public void CreatePlayerUnit(Unit unit)
    {
        _playerUnits.Add(unit);
    }


    public Canvas GetScreenSpaceCanvas()
    {
        return _screenSpaceParentCanvas;
    }
    
    public Canvas GetWorldSpaceCanvas()
    {
        return _worldSpaceParentCanvas;
    }

    public UIBehaviour BuildUI(GameObject uiPrefab, object builder)
    {
        return _uiManager.BuildUI(uiPrefab, builder);
    }
}

