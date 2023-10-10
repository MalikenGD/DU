using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //TODO:
    //mesh/model?
    //name?
    //stats(Scriptable object?) - Atk range, damage, health

    public GridPosition _gridPosition;
    private bool _isDamageable = true;

    private Unit _target;

    private Cell _cell;

    public void InitialSetup(Cell cell)
    {
        SetCurrentCell(cell);
        _gridPosition = _cell.GetGridPosition();
        //faction?
    }
    
    public Cell GetCell()
    {
        return _cell;
    }

    public void SetCurrentCell(Cell newCell)
    {
        _cell?.ClearUnit();
        _cell = newCell;
        _cell.SetUnit(this);
    }

    public void UpdateWorldPosition(Vector3 newWorldPosition)
    {
        transform.position = newWorldPosition;
    }

    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }
}