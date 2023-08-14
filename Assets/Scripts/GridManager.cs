using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Transform unitVisual; // A plane that holds a semi-transparent grid layout to show players where to place units
    [SerializeField] private Transform gridVisual; // DEBUG: Helps me see which grid cells are occupied
    [SerializeField] private Vector2 gridSize;
    private List<GridObject> _gridObjects;
    public static GridManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
        
        //Debug visual objects to show me where each grid square is, and if it's occupied
        _gridObjects = new List<GridObject>();
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                var gridObject = new GridObject {_gridPosition = new GridPosition(i,j)};
                _gridObjects.Add(gridObject);
                var debugUnit = Instantiate(unitVisual,
                    new Vector3(
                        gridVisual.GetComponent<Renderer>().bounds.min.x + gridObject._gridPosition.x + 0.5f,
                        unitVisual.transform.position.y,
                        gridVisual.GetComponent<Renderer>().bounds.min.z + gridObject._gridPosition.z + 0.5f),
                    quaternion.identity).GetComponent<Unit>();

            }
        }
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
        foreach (var gridObject in _gridObjects)
        {
            if (gridObject._gridPosition.Equals(gridPosition))
            {
                gridObject.SetUnit(unit);
                unit.SetGridPosition(gridPosition);
                unit.SetGridObject(gridObject);
            }
        }
    }

    private void ClearUnitAtGridPosition(GridPosition gridPosition)
    {
        foreach (var gridObject in _gridObjects)
        {
            if (gridObject._gridPosition.Equals(gridPosition))
            {
                gridObject.ClearUnit();
            }
        }
    }

    private GridObject GetGridObjectAtPosition(GridPosition gridPosition)
    {
        Vector3 worldPos = ConvertFromGridPositionToWorldPosition(gridPosition);
        
        foreach (var gridObject in _gridObjects)
        {
            if (gridObject._gridPosition.Equals(gridPosition))
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

        foreach (var gridObject in _gridObjects)
        {
            if (gridObject._gridPosition.Equals(gridPosition))
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
        ClearUnitAtGridPosition(unit.GetGridObject()._gridPosition);
        SetUnitAtGridPosition(gridPosition, unit);
    }

    public void UpdateUnitGridPosition(Vector3 worldPosition, Unit unit)
    {
        var gridPosition = ConvertFromWorldPositionToGridPosition(worldPosition);

        ClearUnitAtGridPosition(unit.GetGridObject()._gridPosition);
        SetUnitAtGridPosition(gridPosition, unit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var gridObject in _gridObjects)
            {
                Debug.Log($"Do I have a occupying unit?: {gridObject.AmIOccupied()}");
            }
        }
        
    }
}
