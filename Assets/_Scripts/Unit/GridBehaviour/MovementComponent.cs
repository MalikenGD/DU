using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Unit.Attributes.AttributeSet;
using _Scripts.Unit.GridBehaviour;
using ProjectDawn.Navigation.Hybrid;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Component = _Scripts.Unit.GridBehaviour.Component;

public class MovementComponent : Component, ITickable, ICombatStats
{
    //MovementComponent can also sometimes be responsible or handling collision solutions
    private NavMeshAgent _navMeshAgent;
    private StatMovementSpeed _movementSpeed;
    protected StatAttackRange _attackRange;
    private AgentAuthoring _agentAuthoringComponent;
    private Vector3 _currentDestination;
    
    // Passed in from spawner? Unique to each level? For now hardcoded
    private Vector3 _goalPosition = new Vector3(54.5f, 0.5f, 5.25f); 

    public override void Start()
    {
        priority = Priority.Normal;
        
        base.Start();
    }
    
    public virtual void SetCharacterStats(List<CharacterStat> characterStats)
    {
        
        
        foreach (CharacterStat characterStat in characterStats)
        {
            
            if (_movementSpeed != null && _attackRange != null)
            {
                break;
            }
            switch (characterStat)
            {
                case StatMovementSpeed movementSpeed:
                    _movementSpeed = movementSpeed;
                    break;
                case StatAttackRange attackRange:
                    _attackRange = attackRange;
                    break;
                default:
                    break;
            }
        }

        if (_movementSpeed == null)
        {
            Debug.Log("MovementComponent.SetCharacterStats: _movementSpeed not set.");
        }
        
        if (_attackRange == null)
        {
            Debug.Log("MovementComponent.SetCharacterStats: _attackRange not set.");
        }
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _agentAuthoringComponent = GetComponent<AgentAuthoring>();

    }

    public void BT_MoveToGoal()
    {
        //TODO: Not currently taking into account movementspeed, may have to build my own solution.
        _agentAuthoringComponent.SetDestination(_goalPosition);
    }

    /*public void MoveTo(Vector3 newPosition)
    {
        //Check to see if newPosition is on NavMesh
        NavMeshPath path = new NavMeshPath();
        _navMeshAgent.CalculatePath(newPosition, path);

        if (path.status is NavMeshPathStatus.PathPartial or NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("MovementComponent.MoveTo: newPosition is not a valid point on NavMesh.");
            return;
        }
        
        //TODO: Not currently taking into account movementspeed, may have to build my own solution.
        _agentAuthoringComponent.SetDestination(newPosition);
    }*/

    public void ResetDestination()
    {
        _navMeshAgent.SetDestination(transform.position);
        
        //Bug when moving unit to the navmesh, doesn't recognize it's on navmesh until I do this.
        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;
    }

    public void Tick()
    {
        
        // TODO: After X ticks, update move position to latest position of target/destination
    }

    public void BT_Chase()
    {
        //TODO: Implement chase logic
    }


    
}
