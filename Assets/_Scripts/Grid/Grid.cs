using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grid : MonoBehaviour
{
    [SerializeField] internal GridDataSO gridDataSO;
    private List<Cell> _cells = new List<Cell>();
    private Cell _selectedCell = null;
    private Unit _unitBeingDragged = null;

    public event Action OnGridUpdated;
    public event Action<Cell> OnCellSelected;

    private void Start()
    {
        Vector2 gridSize = gridDataSO.GetGridSize();
        GameObject cellPrefab = gridDataSO.GetCellDataSO().GetCellUIPrefab();

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Cell cell = new Cell(new GridPosition(i, j), cellPrefab);
                _cells.Add(cell);
            }
        }


        CreateGridUI();
    }

    private void CreateGridUI()
    {
        World.Instance.BuildUI(gridDataSO.GetGridUIPrefab(), this);
    }

    public List<Cell> GetCells()
    {
        return _cells;
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
        }
        else
        {
            editedWorldPos.x = Mathf.Ceil(worldPosition.x);
        }

        if (worldPosition.z % 1 < 0.5f)
        {
            editedWorldPos.z = Mathf.Floor(worldPosition.z);
        }
        else
        {
            editedWorldPos.z = Mathf.Ceil(worldPosition.z);
        }
        
        
        
        GridPosition gridPosition = new GridPosition((int)editedWorldPos.x - 51, (int)editedWorldPos.z - 21);
        return gridPosition;
    }

    public void SetUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        foreach (var cell in _cells)
        {
            if (cell.GetGridPosition().Equals(gridPosition))
            {
                cell.SetUnit(unit);
                unit.SetGridPosition(gridPosition);
                unit.SetCurrentCell(cell);
            }
        }
    }
    
    public void SetUnitAtSelectedCell(Unit unit)
    {
        if (_selectedCell == null)
        {
            Debug.LogError("Grid.SetUnitAtGridPosition(unit): Attempting to set unit at null selected cell.");
        }

        if (unit == null)
        {
            Debug.LogError("Grid.SetUnitAtGridPosition(unit): Attempting to set null unit at selected cell.");
        }

        _selectedCell.ClearUnit();
        _selectedCell.SetUnit(unit);
        unit.InitialSetup(_selectedCell); //TODO: Stop unit from setting it's own position, and move it here instead.
                                          //This will break CellUI. 
        _selectedCell = null;
    }

    private void ClearUnitAtGridPosition(GridPosition gridPosition)
    {
        foreach (var cell in _cells)
        {
            if (cell.GetGridPosition().Equals(gridPosition))
            {
                cell.ClearUnit();
            }
        }
    }

    private Cell GetCellAtPosition(GridPosition gridPosition)
    {
        Vector3 worldPos = ConvertFromGridPositionToWorldPosition(gridPosition);
        
        foreach (var cell in _cells)
        {
            if (cell.GetGridPosition().Equals(gridPosition))
            {
                return cell;
            }
        }
        
        Debug.LogError($"Grid.GetCellAtPosition: Could not locate grid object at world position: {worldPos} / grid position: {gridPosition}");
        return null;
    }

    public Cell GetCellAtPosition(Vector3 worldPos)
    {
        GridPosition gridPosition = ConvertFromWorldPositionToGridPosition(worldPos);

        foreach (var cell in _cells)
        {
            if (cell.GetGridPosition().Equals(gridPosition))
            {
                return cell;
            }
        }

        Debug.LogError($"Could not locate grid object at world position: {worldPos} / grid position: {gridPosition}");
        return null;
    }

    public bool IsGridPositionOccupied(GridPosition gridPosition)
    {
        Cell cell = GetCellAtPosition(gridPosition);
        return cell.GetUnit() is not null;
    }

    public void UpdateUnitGridPosition(GridPosition gridPosition, Unit unit)
    {
        ClearUnitAtGridPosition(unit.GetCell().GetGridPosition());
        SetUnitAtGridPosition(gridPosition, unit);
    }

    public void UpdateUnitGridPosition(Vector3 worldPosition, Unit unit)
    {
        var gridPosition = ConvertFromWorldPositionToGridPosition(worldPosition);

        ClearUnitAtGridPosition(unit.GetCell().GetGridPosition());
        SetUnitAtGridPosition(gridPosition, unit);
    }

    internal void SetSelectedCell(Cell selectedCell)
    {
        _selectedCell = selectedCell;
        OnCellSelected?.Invoke(selectedCell);
        //Debug.Log($"SELECTED CELL IS: {cell.GetGridPosition()}");
    }

    internal Cell GetSelectedCell()
    {
        return _selectedCell;
    }

    public bool HasSelectedCell()
    {
        return _selectedCell is not null;
    }

    internal void HandleBeginDragging(object objectBeingDragged)
    {
        bool isObjectBeingDraggedNull = objectBeingDragged == null;
        Cell cell = objectBeingDragged as Cell;
        bool isCellNull = cell == null;

        if (isObjectBeingDraggedNull || isCellNull)
        {
            if (isObjectBeingDraggedNull)
            {
                Debug.LogError("Grid.HandleEndDrag: objectBeingDragged is Null.");
            }
        
            if (isCellNull)
            {
                Debug.LogError("Grid.HandleEndDrag: cell is Null.");
            }

            return;
        }

        _unitBeingDragged = cell.GetUnit();
        
        //TODO: Change material to Transparent for duration of dragging.
    }

    internal void HandleDragging()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Grid.HandleDragging: Camera.main is null.");
            return;
        }
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit);
        _unitBeingDragged.UpdateWorldPosition(new Vector3(hit.point.x, _unitBeingDragged.transform.position.y, hit.point.z));
    }

    public void HandleEndDrag(object objectBeingDragged, Cell highlightedCell)
    {
        bool isObjectBeingDraggedNull = objectBeingDragged == null;
        Cell cell = objectBeingDragged as Cell;
        bool isCellNull = cell == null;
        bool isHighlightedCellNull = highlightedCell == null;

        if (isObjectBeingDraggedNull || isCellNull || isHighlightedCellNull)
        {
            if (isObjectBeingDraggedNull)
            {
                Debug.LogError("Grid.HandleEndDrag: objectBeingDragged is Null.");
            }
        
            if (isCellNull)
            {
                Debug.LogError("Grid.HandleEndDrag: cell is Null.");
            }
        
            if (isHighlightedCellNull)
            {
                Debug.LogError("Grid.HandleEndDrag: highlightedCell is Null.");
            }

            return;
        }


        Unit unit = cell.GetUnit();
        
        if (cell == highlightedCell)
        {
            Vector3 originalPosition = ConvertFromGridPositionToWorldPosition(unit._gridPosition);
            unit.gameObject.transform.position = originalPosition;
            return;
        }
        
        Vector3 newPosition = ConvertFromGridPositionToWorldPosition(highlightedCell.GetGridPosition());
        unit.UpdateWorldPosition(newPosition);
        cell.ClearUnit();
        highlightedCell.SetUnit(unit);
        _unitBeingDragged = null;
        
        //TODO: Change material back from Transparent.
    }
}