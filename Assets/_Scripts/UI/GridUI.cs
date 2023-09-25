using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GridUI : UIBehaviour
{
    private Grid _grid;
    private List<Cell> _cells;
    private List<CellUI> _cellUIs = new List<CellUI>();

    private CellUI _selectedCellUI;
    private Cell _highlightedCell;
    private Unit _unitBeingDragged;
    

    private void Start()
    {
        transform.parent = World.Instance.GetWorldSpaceCanvas().transform;

        _grid = parentObjectWithDataToDisplay as Grid;

        if (_grid is null)
        {
            Debug.Log("GridUI:Start: Grid is null");
            return;
        }
        
        _cells = _grid.GetCells();
        
        
        foreach (Cell cell in _cells)
        {
            //TODO: Unsubscribe from CellUI events. Perhaps on CellUI destruction?
            
            
            Vector3 newWorldPosition = _grid.ConvertFromGridPositionToWorldPosition(cell.GetGridPosition());
            CellUI cellUI = cell.CreateCellUI(newWorldPosition);
            cellUI.transform.parent = gameObject.transform;
            cellUI.OnCellClicked += HandleClick;
            cellUI.OnUnitDragged += HandleDragFinished;
            cellUI.OnUnitDragging += HandleDragging;
            cellUI.OnCellMouseOver += HandleMouseOver;
            cellUI.OnCellMouseExit += HandleMouseExit;
            
            _cellUIs.Add(cellUI);
        }
        
    }

    private void HandleClick(Cell cell, CellUI cellUI)
    {
        //Debug.Log($"Cell clicked at {cell.GetGridPosition()}");
        
        _selectedCellUI?.SetNewState(UIState.Disabled);
        
        _grid.SetSelectedCell(cell);
        _selectedCellUI = cellUI;
        
        cellUI.SetNewState(UIState.Selected);
    }

    private void HandleMouseOver(Cell cell, CellUI cellUI)
    {
        //Debug.Log("Mouse over: " + cell.GetGridPosition());
        _highlightedCell = cell;
        cellUI.SetNewState(UIState.Highlighted);
    }
    
    private void HandleMouseExit(Cell cell, CellUI cellUI)
    {
        if (_grid.GetSelectedCell() == cell)
        {
            return;
        }

        if (_unitBeingDragged is not null)
        {
            return;
        }
        
        cellUI.SetNewState(UIState.Disabled);
    }

    private void HandleDragFinished()
    {
        foreach (CellUI cellUI in _cellUIs)
        {
            //cellUI.SetNewState(cellUI.previousState);
            cellUI.SetNewState(UIState.Disabled);
        }
        
        if (_unitBeingDragged is not null)
        {
            
            if (_unitBeingDragged.GetCell() == _highlightedCell)
            {
                Vector3 originalPosition = _grid.ConvertFromGridPositionToWorldPosition(_unitBeingDragged._gridPosition);
                _unitBeingDragged.gameObject.transform.position = originalPosition;
                return;
            }
            
            _unitBeingDragged.SetCurrentCell(_highlightedCell);
            Vector3 newPosition = _grid.ConvertFromGridPositionToWorldPosition(_highlightedCell.GetGridPosition());
            _unitBeingDragged.UpdateWorldPosition(newPosition);
            _unitBeingDragged = null;
        }
        
        //TODO: Swap units if dragged ontop of another unit
        //TODO: Add VFX on unit drag finished. But how?
    }

    private void HandleDragging(Unit unitBeingDragged)
    {
        if (unitBeingDragged is null)
        {
            return;
        }
        
        foreach (CellUI cellUI in _cellUIs)
        {
            //cellUI.StorePreviousState();
            cellUI.SetNewState(UIState.Highlighted);
        }
        _unitBeingDragged = unitBeingDragged;
        
        _grid.SetSelectedCell(null);
    }

    protected override void UpdateWhenDisabled()
    {
        //When hiding overall GridUI, cycle through children cellUIs to mark them disabled. 
        foreach (CellUI cellUI in _cellUIs)
        {
            cellUI.StorePreviousState();
            cellUI.SetNewState(UIState.Disabled);
        }
    }
}
