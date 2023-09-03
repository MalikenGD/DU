using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GridDataSO", menuName = "ScriptableObjects/Grid/GridData")]
public class GridDataSO : ScriptableObject
{
    public Transform gridPlaneVisual; // A plane that holds a semi-transparent grid layout to show players where to place units
    public Transform debugIndividualCellVisual; // DEBUG: Helps me see which grid cells are occupied
    public Vector2 gridSize;
}
