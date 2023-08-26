using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitCompleteList", menuName = "ScriptableObjects/Unit/UnitCompleteList")]
public class UnitCompleteListSO : ScriptableObject
{
    [SerializeField] private List<UnitDataSO> listOfAllUnitsData;

    public List<UnitDataSO> GetList()
    {
        return listOfAllUnitsData;
    }
}
