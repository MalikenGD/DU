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
    
    private void Start()
    {
        _homePosition = transform.position;
    }

    private void Update()
    {
        UnityEngine.Debug.Log($"Navmesh destination is: {navMeshAgent.destination.ToString()}");
        Debug.Log($"Home position is {_homePosition}");
        Debug.Log($"Current position is: {transform.position}");
    }

    public void SetBehaviourTree(BehaviourTree behaviourTree)
    {
        unitBrain.SetBehaviourTree(behaviourTree);
    }
    
    public void SetCurrentCell(Cell newCell)
    {
        _cell = newCell;
    }

    public void UpdateWorldPosition(Vector3 newWorldPosition)
    {
        Debug.Log("UDPATING WORLD POS");
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
