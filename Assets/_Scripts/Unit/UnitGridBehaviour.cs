using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitGridBehaviour : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;

    private Cell _cell;
    
    private Vector3 _homePosition;

    public event Action OnUnitDragBegin;
    public event Action OnUnitDragEnd;
    
    private void Start()
    {
        _homePosition = transform.position;
    }

    public void UnitDragBegin()
    {
        OnUnitDragBegin?.Invoke();
    }
    public void UnitDragEnd()
    {
        OnUnitDragEnd?.Invoke();
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
