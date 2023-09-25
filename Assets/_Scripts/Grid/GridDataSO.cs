using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GridDataSO", menuName = "ScriptableObjects/Grid/GridData")]
public class GridDataSO : ScriptableObject
{
    [SerializeField] private CellDataSO cellDataSO;
    [SerializeField] private Transform gridPlaneVisual; // A plane that holds a semi-transparent grid layout to show players where to place units
    [SerializeField] private Transform debugIndividualCellVisual; // DEBUG: Helps me see which grid cells are occupied
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private GameObject gridUIPrefab;

    public Vector2 GetGridSize()
    {
        return gridSize;
    }

    public GameObject GetGridUIPrefab()
    {
        return gridUIPrefab;
    }

    public CellDataSO GetCellDataSO()
    {
        return cellDataSO;
    }

}
