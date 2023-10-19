using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;

public class UnitGridBehaviour : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private UnitBrain unitBrain;

    private Cell _cell;
    
    private Vector3 _homePosition;

    public event Action OnUnitInitialized;
    
    private void Start()
    {
        _homePosition = transform.position;
    }

    private void Update()
    {
        /*UnityEngine.Debug.Log($"Navmesh destination is: {navMeshAgent.destination.ToString()}");
        Debug.Log($"Home position is {_homePosition}");
        Debug.Log($"Current position is: {transform.position}");*/
    }

    public void InitializeUnit(BehaviourTree behaviourTree)
    {
        bool isNavmeshAgentNull = navMeshAgent == null;

        if (isNavmeshAgentNull)
        {
            Debug.LogError("UnitGridBehaviour.InitializeUnit: NavmeshAgent is null.");
            return;
        }

        navMeshAgent.enabled = false;
        navMeshAgent.enabled = true;
        
        SetBehaviourTree(behaviourTree);
        OnUnitInitialized?.Invoke();
    }

    private void SetBehaviourTree(BehaviourTree behaviourTree)
    {
        unitBrain.SetBehaviourTree(behaviourTree);
    }
    
    public void SetCurrentCell(Cell newCell)
    {
        _cell = newCell;
    }

    public void UpdateWorldPosition(Vector3 newWorldPosition)
    {
        transform.position = newWorldPosition;
        _homePosition = newWorldPosition;
    }
    
    public Cell GetCell()
    {
        return _cell;
    }
    
    public void SetNavMeshAgentActive(bool value)
    {
        navMeshAgent.enabled = value;
    }
    
    public void OnGameStateChanged(GameState obj)
    {
        ResetToDefault();
    }

    private void ResetToDefault()
    {
        //Revert to original position
        transform.position = _homePosition;
        //Heal to full
        
        //Re-enable if dead
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
}
