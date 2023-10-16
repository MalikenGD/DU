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
    private object _objectBeingDragged;
    

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
            
            //TODO: Decouple gameplay naming from UI logic. Click, Drag, All fine. Unit/Cell not so much.
            Vector3 newWorldPosition = _grid.ConvertFromGridPositionToWorldPosition(cell.GetGridPosition());
            CellUI cellUI = cell.CreateCellUI(newWorldPosition);
            cellUI.transform.parent = gameObject.transform;
            cellUI.OnMouseClick += HandleMouseClick;
            cellUI.OnEndDragging += HandleEndDrag;
            cellUI.OnBeginDragging += HandleBeginDragging;
            cellUI.OnPointerEntering += HandlePointerEntering;
            cellUI.OnPointerExiting += HandlePointerExiting;
            cellUI.OnDragging += HandleDragging;
            
            _cellUIs.Add(cellUI);
        }
        
    }

    private void HandleMouseClick(Cell cell, CellUI cellUI)
    {
        if (_selectedCellUI != null)
        {
            _selectedCellUI?.SetNewState(UIState.Disabled);
        }
        
        _grid.SetSelectedCell(cell);
        _selectedCellUI = cellUI;
        
        cellUI.SetNewState(UIState.Selected);
    }

    private void HandlePointerEntering(Cell cell, CellUI cellUI)
    {
        //Debug.Log("Mouse over: " + cell.GetGridPosition());
        _highlightedCell = cell;
        cellUI.SetNewState(UIState.Highlighted);
    }
    
    private void HandlePointerExiting(Cell cell, CellUI cellUI)
    {
        if (_grid.GetSelectedCell() == cell)
        {
            return;
        }

        if (_objectBeingDragged is not null)
        {
            return;
        }
        
        cellUI.SetNewState(UIState.Disabled);
    }
    private void HandleBeginDragging(object objectBeingDragged)
    {
        if (objectBeingDragged is null)
        {
            return;
        }
        
        foreach (CellUI cellUI in _cellUIs)
        {
            //cellUI.StorePreviousState();
            cellUI.SetNewState(UIState.Highlighted);
        }
        _objectBeingDragged = objectBeingDragged;
        
        _grid.SetSelectedCell(null);
        
        _grid.HandleBeginDragging(_objectBeingDragged);
    }
    
    private void HandleDragging()
    {
        _grid.HandleDragging();
    }
    
    private void HandleEndDrag(PointerEventData pointerEventData)
    {
        foreach (CellUI cellUI in _cellUIs)
        {
            //cellUI.SetNewState(cellUI.previousState);
            cellUI.SetNewState(UIState.Disabled);
        }
        
        if (_objectBeingDragged is null)
        {
            return;
        }
        
        
        _grid.HandleEndDrag(_objectBeingDragged, _highlightedCell);

        _objectBeingDragged = null;

        //Swap units if dragged ontop of another unit
        //Add VFX on unit drag finished. But how?
    }

    protected override void UpdateWhenDisabled()
    {
        //Does this happen every update frame?
        
        //When hiding overall GridUI, cycle through children cellUIs to mark them disabled. 
        foreach (CellUI cellUI in _cellUIs)
        {
            cellUI.StorePreviousState();
            cellUI.SetNewState(UIState.Disabled);
        }
    }
}
