using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    public GridPosition _gridPosition;

    private Unit _occupyingUnit;
    

    public void SetUnit(Unit unitToSet)
    {
        _occupyingUnit = unitToSet;
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
