using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    [SerializeField] private float heightModifier;
    [SerializeField] private float radiusOfSphereCast; // This should be tied to attack range, no?
    private Vector3 originPosition;

    private void Start()
    {
        List<GameObject> gameObjects = SphereCast();
        foreach (GameObject Unit in gameObjects)
        {
            Debug.Log(Unit.name);
        }

        Debug.Log($"Count: {gameObjects.Count}");
        
    }

    //Spherecast from above to below, storing units in a sorted list.
    public List<GameObject> SphereCast()
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
