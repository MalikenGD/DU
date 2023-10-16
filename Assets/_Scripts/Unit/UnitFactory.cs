using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class UnitFactory
{
    public Unit BuildUnit(GameObject unitPrefab, Vector3 spawningPosition, Transform parentTransform)
    {
        GameObject unitGameObject = GameObject.Instantiate(unitPrefab, spawningPosition, quaternion.identity);
        unitGameObject.transform.parent = parentTransform;
        Unit unit = unitGameObject.GetComponent<Unit>();

        if (unit == null)
        {
            Debug.LogError("UnitFactory.BuildUnit: unitGameObject has no Unit");
            return null;
        }
        
        return unit;
    }
}
