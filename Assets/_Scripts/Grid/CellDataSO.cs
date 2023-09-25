using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "CellDataSO", menuName = "ScriptableObjects/Grid/CellData")]
public class CellDataSO : ScriptableObject
{
    [SerializeField] private GameObject _cellUIPrefab;

    public GameObject GetCellUIPrefab()
    {
        return _cellUIPrefab;
    }
}
