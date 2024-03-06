using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MovementComponent : MonoBehaviour
{
    //MovementComponent can also sometimes be responsible or handling collision solutions
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 newPosition)
    {
        //Check to see if newPosition is on NavMesh
        NavMeshPath path = new NavMeshPath();
        _navMeshAgent.CalculatePath(newPosition, path);

        if (path.status is NavMeshPathStatus.PathPartial or NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("MovementComponent.MoveTo: newPosition is not a valid point on NavMesh.");
            return;
        }
        
        _navMeshAgent.SetDestination(newPosition);
    }

    public void ResetDestination()
    {
        _navMeshAgent.SetDestination(transform.position);
        
        //Bug when moving unit to the navmesh, doesn't recognize it's on navmesh until I do this.
        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;
    }

    public bool BehindTarget()
    {
        return true;
    }
}
