using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridDataSO gridDataSO;
    private Grid _grid;
    private GridObject _selectedCell = null;

    public event Action OnGridUpdated;
    public event Action<GridObject> OnGridCellSelected;

    private void Start()
    {
        _grid = new Grid(gridDataSO.GetGridSize());

        foreach (GridObject gridObject in _grid.GetGridObjectList())
        {
            UIBehaviour gridUIObject = World.Instance.BuildUI(gridDataSO.GetGridUIPrefab(), this);
            //gridUIObject.GetComponent<GridUI>().OnGridUIButtonClicked += SetGridCellSelected;
        }

    }

    public GridObject GetGridObjectAssignment(GameObject buttonGameObject)
    {
        foreach (GridObject gridObject in _grid.GetGridObjectList())
        {
            if (gridObject.GetAssignedButton() is null)
            {
                gridObject.SetButton(buttonGameObject);
                return gridObject;
            }
        }

        Debug.LogError("GridManager:GetGridObjectAssignment: No available GridObject to be matched with a Grid Button");
        return null;
    }
    
    public Vector3 FormatWorldPositionForGridSnapping(Vector3 dirtyWorldPosition)
    {
        //Takes the world position from mouse or other sources and rounds it to snap with center of grid cells
        Vector3 cleanWorldPosition = Vector3.zero;
        GridPosition temporaryConversion;
        temporaryConversion = ConvertFromWorldPositionToGridPosition(dirtyWorldPosition);
        cleanWorldPosition = ConvertFromGridPositionToWorldPosition(temporaryConversion);
        return cleanWorldPosition;
    }




    public Vector3 ConvertFromGridPositionToWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(51f + gridPosition.x, 0, 21f + gridPosition.z);
    }
    
    public GridPosition ConvertFromWorldPositionToGridPosition(Vector3 worldPosition)
    {
        Vector3 editedWorldPos = new Vector3();
        if (worldPosition.x % 1 < 0.5f)
        {
            editedWorldPos.x = Mathf.Floor(worldPosition.x);
        } else editedWorldPos.x = Mathf.Ceil(worldPosition.x);

        if (worldPosition.z % 1 < 0.5f)
        {
            editedWorldPos.z = Mathf.Floor(worldPosition.z);
        }
        else editedWorldPos.z = Mathf.Ceil(worldPosition.z);
        
        
        
        GridPosition gridPosition = new GridPosition((int)editedWorldPos.x - 51, (int)editedWorldPos.z - 21);
        return gridPosition;
    }

    public void SetUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        foreach (var gridObject in _grid.GetGridObjectList())
        {
            if (gridObject.GetGridPosition().Equals(gridPosition))
            {
                gridObject.SetUnit(unit);
                unit.SetGridPosition(gridPosition);
                unit.SetGridObject(gridObject);
            }
        }
    }

    private void ClearUnitAtGridPosition(GridPosition gridPosition)
    {
        foreach (var gridObject in _grid.GetGridObjectList())
        {
            if (gridObject.GetGridPosition().Equals(gridPosition))
            {
                gridObject.ClearUnit();
            }
        }
    }

    private GridObject GetGridObjectAtPosition(GridPosition gridPosition)
    {
        Vector3 worldPos = ConvertFromGridPositionToWorldPosition(gridPosition);
        
        foreach (var gridObject in _grid.GetGridObjectList())
        {
            if (gridObject.GetGridPosition().Equals(gridPosition))
            {
                return gridObject;
            }
        }
        
        Debug.LogError($"Could not locate grid object at world position: {worldPos} / grid position: {gridPosition}");
        return null;
    }

    public GridObject GetGridObjectAtPosition(Vector3 worldPos)
    {
        GridPosition gridPosition = ConvertFromWorldPositionToGridPosition(worldPos);

        foreach (var gridObject in _grid.GetGridObjectList())
        {
            if (gridObject.GetGridPosition().Equals(gridPosition))
            {
                return gridObject;
            }
        }

        Debug.LogError($"Could not locate grid object at world position: {worldPos} / grid position: {gridPosition}");
        return null;
    }

    public bool IsGridPositionOccupiedAlready(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridObjectAtPosition(gridPosition);
        return gridObject.AmIOccupied();
    }

    public void UpdateUnitGridPosition(GridPosition gridPosition, Unit unit)
    {
        ClearUnitAtGridPosition(unit.GetGridObject().GetGridPosition());
        SetUnitAtGridPosition(gridPosition, unit);
    }

    public void UpdateUnitGridPosition(Vector3 worldPosition, Unit unit)
    {
        var gridPosition = ConvertFromWorldPositionToGridPosition(worldPosition);

        ClearUnitAtGridPosition(unit.GetGridObject().GetGridPosition());
        SetUnitAtGridPosition(gridPosition, unit);
    }

    internal void SetGridCellSelected(GridObject gridObject)
    {
        OnGridCellSelected?.Invoke(gridObject);
        _selectedCell = gridObject;
        Debug.Log($"SELECTED CELL IS: {gridObject.GetGridPosition()}");
    }


}

public class Grid
{
    private List<GridObject> _gridObjects;

    public Grid(Vector2 gridSize)
    {
        _gridObjects = new List<GridObject>();
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                var gridObject = new GridObject(new GridPosition(i,j));
                _gridObjects.Add(gridObject);
            }
        }
    }

    public List<GridObject> GetGridObjectList()
    {
        return _gridObjects;
    }
}

