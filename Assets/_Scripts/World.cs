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
    private UnitFactory _unitFactory;

    public UIManager uiManager
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
    
    public UnitFactory unitFactory
    {
        get
        {
            return _unitFactory;
        }
        private set
        {
            _unitFactory = value;
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
        _unitFactory = new UnitFactory();
        
        GameObject gameModeObject = Instantiate(gameModePrefab, transform);
        if (gameModeObject != null)
        {
            _gameMode = gameModeObject.GetComponent<GameMode>();
        }
        else
        {
            Debug.LogError("World.Awake: GameMode not valid.");
        }
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

    private void Update()
    {
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Debug.Log("toggling");
            _uiManager.ToggleAllUIObjects();
        }
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

    public Unit BuildUnit(GameObject unitPrefab, Vector3 spawningPosition, int faction)
    {
        //faction 0 = enemy unit, faction 1 = player unit
        Unit unit = _unitFactory.BuildUnit(unitPrefab, spawningPosition, gameObject.transform);

        if (unit == null)
        {
            Debug.LogError("World.BuildUnit: Unit created is null.");
            return null;
        }
        
        if (faction == 0)
        {
            _enemyUnits.Add(unit);
        }
        else if (faction == 1)
        {
            _playerUnits.Add(unit);
        }
        else
        {
            Debug.LogError("World.BuildUnit: Faction selection out of range.");
        }

        return unit;
    }
}

