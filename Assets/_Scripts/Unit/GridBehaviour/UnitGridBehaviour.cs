using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

public class UnitGridBehaviour : MonoBehaviour
{    
    
    private GlobalBlackboard _globalBlackboard;
    private Variable _targets;

    private NavMeshAgent _navMeshAgent;

    private Cell _cell;
    private Vector3 _homePosition;

    //Unused?
    public event Action OnUnitDragBegin;
    public event Action OnUnitDragEnd;
    
    private void Start()
    {
        _globalBlackboard = FindObjectOfType<GlobalBlackboard>();
        _targets = _globalBlackboard.GetVariable("_unitsOnNavmesh");
        _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        
        _homePosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        List<GameObject> units = new List<GameObject>();
        units = _targets.value as List<GameObject>;

        if (units == null)
        {
            Debug.LogError("UnitGridBehaviour.OnTriggerEnter: _targets.value is Null.");
            return;
        }

        Unit unit = GetComponent<Unit>();

        if (!units.Contains(unit.gameObject))
        {
            units.Add(unit.gameObject);
        }
    }

    public void UnitDragBegin()
    {
        _navMeshAgent.enabled = false;
        //OnUnitDragBegin?.Invoke();
    }
    public void UnitDragEnd()
    {
        _navMeshAgent.enabled = true;
        //OnUnitDragEnd?.Invoke();
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
