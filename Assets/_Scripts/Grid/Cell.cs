using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private GridPosition _gridPosition;
    private GameObject _cellUIPrefab;
    private Unit _occupyingUnit;


    public Cell(GridPosition gridPosition, GameObject cellUIPrefab)
    {
        _gridPosition = gridPosition;
        _cellUIPrefab = cellUIPrefab;
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
    
    public CellUI CreateCellUI(Vector3 worldPosition)
    {
        UIBehaviour cellUIBehaviour = World.Instance.BuildUI(_cellUIPrefab, this);
        CellUI cellUI = cellUIBehaviour as CellUI;
        cellUI.Initialize(worldPosition);

        return cellUI;
    }
}
