using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : IUIObject
{
    
    //TODO: I should probably hold a reference to the GridManager not Grid
    //TODO: This is going to be a world space UI, look into that.
    private Grid _grid;
    private GameObject _uiGameObject;
    public GridUI(Grid grid, GameObject uiGameObject)
    {
        _grid = grid;
        _uiGameObject = uiGameObject;
    }




    public GameObject GetUIGameObject()
    {
        return _uiGameObject;
    }
}
