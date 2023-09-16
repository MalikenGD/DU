using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GridUI : UIBehaviour
{
    
    //TODO: I should probably hold a reference to the GridManager not Grid
    //TODO: This is going to be a world space UI, look into that.
    private GridManager _gridManager;
    private GameObject _buttonGameObject;
    private GridObject _gridObject;

    private void Start()
    {
        transform.parent = World.Instance.GetWorldSpaceCanvas().transform;
        _gridManager = parentObjectWithDataToDisplay as GridManager;
        _buttonGameObject = GetComponentInChildren<Button>().gameObject;
        _gridObject = _gridManager.GetGridObjectAssignment(_buttonGameObject);
        
        Vector3 worldPosition = _gridManager.ConvertFromGridPositionToWorldPosition(_gridObject.GetGridPosition());
        worldPosition = new Vector3(worldPosition.x, worldPosition.y + 0.01f, worldPosition.z);
        _buttonGameObject.transform.position = worldPosition;
    }

    public void HandleClickLogic()
    {
        _gridManager.SetGridCellSelected(_gridObject);
    }


    private void Update()
    {
        
    }
}
