using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    /*//TODO: Refactor for multiplayer
    //Create and fill a new list of units for each player, and enemy? This should be on the server..
    //Dictionary won't work for MP as is, would need the key to be some sort of playerID instead of faction.

    private Dictionary<Unit.Faction, List<Unit>> _unitListDictionary;
    public Unit _selectedUnit;
    private Vector3 _selectedUnitOriginalPosition;

    public static UnitManager Instance;

    private void OnEnable()
    {
        InputManager.OnClickedUnit += HandleUnitSelectionLogic;
        InputManager.OnMouseHoldingFinished += UpdateUnitGridPosition;
        InputManager.OnMouseHoldingFinished += ClearUnitSelection;
        InputManager.OnMouseHolding += UpdateUnitWorldPosition;
    }

    private void OnDisable()
    {
        InputManager.OnClickedUnit -= HandleUnitSelectionLogic;
        InputManager.OnMouseHoldingFinished -= UpdateUnitGridPosition;
        InputManager.OnMouseHoldingFinished -= ClearUnitSelection;
        InputManager.OnMouseHolding -= UpdateUnitWorldPosition;
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
    }

    private void Start()
    {
        _unitListDictionary = new Dictionary<Unit.Faction, List<Unit>>();
    }

    //TODO: Implement spawning and adding to dictionary (currently mobs are placed manually for testing
    public void AddUnitToDictionary(Unit.Faction faction, Unit unitToAdd)
    {
        if (_unitListDictionary.ContainsKey(faction))
        {
            _unitListDictionary[faction].Add(unitToAdd);
        }
        else
        {
            //If dictionary doesn't have that unit Type in it's list, create it and add unit to it
            _unitListDictionary[faction] = new List<Unit> {unitToAdd};
        }
    }

    private void HandleUnitSelectionLogic(Unit unit)
    {
        if (unit.GetFaction() == Unit.Faction.Enemy)
        {
            return;
        }
        
        if (_selectedUnit == null)
        {
            _selectedUnit = unit;
        }
    }

    private void ClearUnitSelection()
    {
        _selectedUnit = null;
    }

    private void SetUnitOriginalPosition(Vector3 originalPosition)
    {
        _selectedUnitOriginalPosition = originalPosition;
    }

    private void UpdateUnitWorldPosition(Vector3 newWorldPosition)
    {
        if (_selectedUnit)
        {
            var selectedUnitTransform = _selectedUnit.gameObject.transform;
            if (_selectedUnitOriginalPosition == Vector3.zero)
            {
                SetUnitOriginalPosition(GridManager.Instance.FormatWorldPositionForGridSnapping(selectedUnitTransform.position));
            }
            selectedUnitTransform.position = newWorldPosition;
        }
    }

    private void UpdateUnitGridPosition()
    {
        if (!_selectedUnit) return;
        
        var gridManager = GridManager.Instance;

        var unitPosition = _selectedUnit.transform.position;
        var unitDestinationGridPosition =
            gridManager.ConvertFromWorldPositionToGridPosition(_selectedUnit.transform.position);

        if (gridManager.IsGridPositionOccupiedAlready(unitDestinationGridPosition))
        {
            _selectedUnit.transform.position = _selectedUnitOriginalPosition;
            _selectedUnitOriginalPosition = Vector3.zero;
            return;
        }

        gridManager.UpdateUnitGridPosition(unitDestinationGridPosition, _selectedUnit);
        UpdateUnitWorldPosition(gridManager.FormatWorldPositionForGridSnapping(unitPosition));
        _selectedUnitOriginalPosition = Vector3.zero;
    }*/
}
