using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    [SerializeField] private float heightModifier = 2.5f;
    [SerializeField] private float radiusOfSphereCast = 1f; // This should be tied to attack range, no?
    private Vector3 originPosition;

    public Unit EvaluateTarget(Unit currentTarget)
    {
        GameObject closestUnit = null;

        foreach (GameObject unit in SphereCast())
        {
            if (unit.GetComponent<Unit>().GetFaction() == currentTarget.GetFaction())
            {
                closestUnit = unit;
                break;
            }
        }
        float distanceToCurrentTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        float distanceToClosestUnit = Vector3.Distance(transform.position, closestUnit.transform.position);


        return distanceToClosestUnit < distanceToCurrentTarget ? closestUnit.GetComponent<Unit>() : currentTarget;
    }

    //Spherecast from above to below, storing units in a sorted list.
    //Excludes self
    private List<GameObject> SphereCast()
    {
        List<GameObject> unitsHit = new List<GameObject>();
        List<GameObject> unitsOrderedByDistance = new List<GameObject>();
        Vector3 position = transform.position;
        originPosition = new Vector3(position.x, position.y + (2 * heightModifier), position.z);
        
        foreach (RaycastHit raycastHit in Physics.SphereCastAll(originPosition, radiusOfSphereCast, Vector3.down))
        {
            if (raycastHit.collider.gameObject == gameObject)
            {
                continue;
            }
            if (raycastHit.collider.CompareTag("Unit"))
            {
                unitsHit.Add(raycastHit.collider.gameObject);
            }
        }
        
        //Order by distance
        if (unitsHit.Count > 0)
        {
            unitsOrderedByDistance = unitsHit.OrderBy(go => Vector3.Distance(go.transform.position, position)).ToList();
        }
        
        return unitsHit.Count > 0 ? unitsOrderedByDistance : null;
    }
    
    void OnDrawGizmos()
    {
        originPosition = new Vector3(transform.position.x, transform.position.y + (2 * heightModifier), transform.position.z);
        Gizmos.color = Color.red;
        //Debug.Log(originPosition);
        //Debug.Log(heightModifier);
        Gizmos.DrawSphere(originPosition, radiusOfSphereCast);
    }
}
