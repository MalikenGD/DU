using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridPosition _gridPosition;
    private GameObject _button = null;
    private Unit _occupyingUnit;


    public GridObject(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public GameObject GetAssignedButton()
    {
        return _button;
    }

    public void SetButton(GameObject buttonGameObject)
    {
        _button = buttonGameObject;
    }
    public void SetUnit(Unit unitToSet)
    {
        _occupyingUnit = unitToSet;
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    public void ClearUnit()
    {
        _occupyingUnit = null;
    }

    public Unit GetUnit()
    {
        return _occupyingUnit;
    }

    public bool AmIOccupied()
    {
        return _occupyingUnit != null;
    }
}
