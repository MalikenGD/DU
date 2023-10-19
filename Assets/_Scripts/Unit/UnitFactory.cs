using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class UnitFactory
{
    public Unit BuildUnit(GameObject unitPrefab, Transform parentTransform)
    {
        GameObject unitGameObject = GameObject.Instantiate(unitPrefab, parentTransform, true);
        Unit unit = unitGameObject.GetComponent<Unit>();

        if (unit == null)
        {
            Debug.LogError("UnitFactory.BuildUnit: unitGameObject has no Unit");
            return null;
        }
        
        return unit;
    }
    
    
    public void DestroyUnit(Unit unit)
    {
        if (unit is null)
        {
            Debug.LogError("UnitFactory.DestroyUnit: Unit is null.");
            return;
        }
        //UnSubscribeFromEvents(unit); // TODO: Figure out a way to unsubscribe from all events it may be subscribed to.
        GameObject.Destroy(unit);
    }
}
