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
        if (gridDataSO == null)
        {
            Debug.LogError("Grid.Start: GridDataSO is Null.");   
        }
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
    
    /*public Vector3 FormatWorldPositionForGridSnapping(Vector3 dirtyWorldPosition)
    {
        //Takes the world position from mouse or other sources and rounds it to snap with center of grid cells
        Vector3 cleanWorldPosition = Vector3.zero;
        GridPosition temporaryConversion;
        temporaryConversion = ConvertFromWorldPositionToGridPosition(dirtyWorldPosition);
        cleanWorldPosition = ConvertFromGridPositionToWorldPosition(temporaryConversion);
        return cleanWorldPosition;
    }*/
    
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

    /*public void SetUnitAtGridPosition(GridPosition gridPosition, Unit unit)
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
    }*/
    
    public void SetUnitAtSelectedCell(Unit unit)
    {
        bool isSelectedCellNull = _selectedCell == null;
        bool isUnitNull = unit == null;

        if (!unit.TryGetComponent<UnitGridBehaviour>(out UnitGridBehaviour unitGridBehaviour))
        {
            Debug.LogError("Grid.SetUnitAtSelectedCell: Unit has no UnitGridBehaviour.");
        }

        if (isSelectedCellNull || isUnitNull)
        {
            if (isSelectedCellNull)
            {
                Debug.LogError("Grid.SetUnitAtGridPosition(unit): Attempting to set unit at null selected cell.");
            }

            if (isUnitNull)
            {
                Debug.LogError("Grid.SetUnitAtGridPosition(unit): Attempting to set null unit at selected cell.");
            }
            return;
        }

        _selectedCell.ClearUnit();
        _selectedCell.SetUnit(unit);
        unitGridBehaviour.SetCurrentCell(_selectedCell);
                                          
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
        
        Debug.LogError($"Grid.GetCellAtPosition(GridPosition): Could not locate grid object at world position: {worldPos} / grid position: {gridPosition}");
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

        Debug.LogError($"Grid.GetCellAtPosition(Vector3): Could not locate grid object at world position: {worldPos} / grid position: {gridPosition}");
        return null;
    }

    /*public bool IsGridPositionOccupied(GridPosition gridPosition)
    {
        Cell cell = GetCellAtPosition(gridPosition);
        return cell.GetUnit() is not null;
    }*/

    /*public void UpdateUnitGridPosition(GridPosition gridPosition, Unit unit)
    {
        ClearUnitAtGridPosition(unit.GetCell().GetGridPosition());
        SetUnitAtGridPosition(gridPosition, unit);
    }*/

    /*public void UpdateUnitGridPosition(Vector3 worldPosition, Unit unit)
    {
        var gridPosition = ConvertFromWorldPositionToGridPosition(worldPosition);

        ClearUnitAtGridPosition(unit.GetCell().GetGridPosition());
        SetUnitAtGridPosition(gridPosition, unit);
    }*/

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
        
        if (!_unitBeingDragged.TryGetComponent<UnitGridBehaviour>(out UnitGridBehaviour unitGridBehaviour))
        {
            Debug.LogError("Grid.SetUnitAtSelectedCell: Unit has no UnitGridBehaviour.");
            return;
        }

        //unitGridBehaviour.UnitDragBegin();
        unitGridBehaviour.SetNavMeshAgentActive(false); // Turn off navmesh agent to prevent collision while dragging.
        unitGridBehaviour.SetCurrentCell(cell);
        
        //TODO: Change material to Transparent for duration of dragging.
    }

    internal void HandleDragging()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Grid.HandleDragging: Camera.main is null.");
            return;
        }
        
        if (!_unitBeingDragged.TryGetComponent<UnitGridBehaviour>(out UnitGridBehaviour unitGridBehaviour))
        {
            Debug.LogError("Grid.SetUnitAtSelectedCell: Unit has no UnitGridBehaviour.");
            return;
        }
        
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit);
        unitGridBehaviour.UpdateWorldPosition(new Vector3(hit.point.x, _unitBeingDragged.transform.position.y, hit.point.z));
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
        
        
        Vector2 gridSize = gridDataSO.GetGridSize();
        GridPosition unitGridPosition = ConvertFromWorldPositionToGridPosition(_unitBeingDragged.transform.position);
        
        //If unit dragged out of bounds
        if (unitGridPosition.x > gridSize.x || unitGridPosition.z > gridSize.y || unitGridPosition.x < 0 || unitGridPosition.z < 0)
        {
            ResetUnitPosition(_unitBeingDragged);
            _unitBeingDragged = null;
            return;
        }

        //If dragged on-top of another unit/occupied cell
        if (highlightedCell.GetUnit() != null)
        {
            cell.ClearUnit();
            cell.SetUnit(highlightedCell.GetUnit());
            
            highlightedCell.ClearUnit();
            highlightedCell.SetUnit(_unitBeingDragged);
            
            UpdateUnitPosition(cell.GetUnit(), cell);
            UpdateUnitPosition(_unitBeingDragged, highlightedCell);
        }
        else
        {
            UpdateUnitPosition(_unitBeingDragged, highlightedCell);
            cell.ClearUnit();
            highlightedCell.SetUnit(_unitBeingDragged);
        }
        
        if (!_unitBeingDragged.TryGetComponent<UnitGridBehaviour>(out UnitGridBehaviour unitGridBehaviour))
        {
            Debug.LogError("Grid.SetUnitAtSelectedCell: Unit has no UnitGridBehaviour.");
            return;
        }
        
        unitGridBehaviour.SetNavMeshAgentActive(true);
        _unitBeingDragged = null;
        
        //TODO: Change material back from Transparent.
    }
    
    private void UpdateUnitPosition(Unit unit, Cell newCell)
    {
        if (!unit.TryGetComponent<UnitGridBehaviour>(out UnitGridBehaviour unitGridBehaviour))
        {
            Debug.LogError("Grid.SetUnitAtSelectedCell: Unit has no UnitGridBehaviour.");
            return;
        }
        
        Vector3 newPosition = ConvertFromGridPositionToWorldPosition(newCell.GetGridPosition());
        unitGridBehaviour.UpdateWorldPosition(newPosition);
    }

    private void ResetUnitPosition(Unit unit)
    {
        if (!unit.TryGetComponent<UnitGridBehaviour>(out UnitGridBehaviour unitGridBehaviour))
        {
            Debug.LogError("Grid.SetUnitAtSelectedCell: Unit has no UnitGridBehaviour.");
            return;
        }
        
        Vector3 originalPosition = ConvertFromGridPositionToWorldPosition(unitGridBehaviour.GetCell().GetGridPosition());
        unit.gameObject.transform.position = originalPosition;
    }
}