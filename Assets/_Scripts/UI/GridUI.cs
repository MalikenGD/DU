using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : UIBehaviour
{
    
    //TODO: I should probably hold a reference to the GridManager not Grid
    //TODO: This is going to be a world space UI, look into that.
    private Grid _grid;
    

    public GameObject GetUIGameObject()
    {
        return gameObject;
    }

    public void Init(object builder)
    {
        _grid = builder as Grid;
    }
}
