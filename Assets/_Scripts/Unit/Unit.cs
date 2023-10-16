using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;

    private Vector3 _homePosition;
    private int _health;
    private Unit _target;
    private Cell _cell;

    private void Start()
    {
        _homePosition = transform.position;
    }


    public void SetCurrentCell(Cell newCell)
    {
        //_cell?.ClearUnit();
        _cell = newCell;
        //_cell.SetUnit(this);
    }
    
    public Cell GetCell()
    {
        return _cell;
    }

    public void UpdateWorldPosition(Vector3 newWorldPosition)
    {
        transform.position = newWorldPosition;
        _homePosition = newWorldPosition;
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