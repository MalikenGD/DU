using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Unit.Attributes.AttributeSet;
using _Scripts.Unit.GridBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using Component = _Scripts.Unit.GridBehaviour.Component;

public class TargetingComponent : Component, ITickable, ICombatStats
{
    public event Action<Unit> OnTargetUpdated;
    
    [SerializeField] private float sphereCastHeightModifier = 2.5f;
    protected Unit _currentTarget = null;
    private bool _currentTargetOutOfRange;
    private float _radiusOfSphereCast;
    protected CombatClass _combatClass;
    protected StatAttackRange AttackRange;
    private float _attackRangeWiggleRoom = 0.5f; // Wiggle room, added to attack range. Change how it's set? TODO:
    private int _targetingRange = 15; // TODO: Set this somehow.
    protected int _targetSwitchDelay = 1;
    protected float _timeUntilNextTargetSwitch = 0f;

    public override void Start()
    {
        priority = Priority.Immediate;
        
        base.Start();
    }

    public virtual void SetCharacterStats(List<CharacterStat> characterStats)
    {
        foreach (CharacterStat characterStat in characterStats)
        {
            if (characterStat is not StatAttackRange statAttackRange)
            {
                continue;
            }

            AttackRange = statAttackRange;
            break;
        }

        if (AttackRange == null)
        {
            Debug.Log("AssassinTargetingComponent.SetCharacterStats: _attackRange not set.");
        }
    }


    public bool BT_IsCurrentTargetInAttackRange()
    {
        //Has target died or otherwise been disabled
        if (_currentTarget == null || _currentTarget.gameObject.activeInHierarchy == false)
        {
            return false;
        }
        
        return Vector3.Distance
               (_currentTarget.transform.position, transform.position) <= (AttackRange.Value + _attackRangeWiggleRoom);
    }

    //Spherecast from above to below, storing units in a sorted list, excluding self.
    //Sorts by range to self
    //Maybe change to overlap sphere instead? 
    //Maybe pull out into a Sensor component? TODO:
    public List<Unit> GetSortedEnemiesInTargetRange()
    {
        _radiusOfSphereCast = _targetingRange;
        List<Unit> unitsHit = new List<Unit>();
        Vector3 myPosition = transform.position;
        Vector3 sphereCastStartPosition = new Vector3(myPosition.x, myPosition.y + (2 * sphereCastHeightModifier), myPosition.z);
        
        int myFaction = GetComponent<Unit>().GetFaction();
        
        foreach (RaycastHit raycastHit in Physics.SphereCastAll(sphereCastStartPosition, _radiusOfSphereCast, Vector3.down))
        {
            if (raycastHit.collider.gameObject == gameObject)
            {
                continue;
            }

            if (!raycastHit.collider.CompareTag("Unit"))
            {
                continue;
            }
            
            Unit unit = raycastHit.collider.GetComponent<Unit>();

            if (unit == null)
            {
                Debug.Log($"TargetingComponent.SphereCast: Object with 'Unit' tag, has a null 'Unit' component.");
                return null;
            }
            // Is other unit (and parents) active in scene
            if (unit.gameObject.activeInHierarchy)
            {
                // Is other unit a part of my faction/team
                if (unit.GetFaction() == myFaction)
                {
                    continue;
                }
            }
                
            unitsHit.Add(raycastHit.collider.gameObject.GetComponent<Unit>());
        }
        
        // Sort list by Range to self
        if (unitsHit.Count > 0)
        {
            unitsHit = unitsHit.OrderBy(unit => Vector3.Distance(unit.transform.position, myPosition)).ToList();
        }

        return unitsHit;
    }

    protected virtual Unit EvaluateAndReturnNewTarget(List<Unit> sortedUnitsWithinTargetRange)
    {
        Unit newTarget = null;

        if (sortedUnitsWithinTargetRange.Count <= 0)
        {
            Debug.Log("TargetingComponent.EvaluateAndReturnNewTarget: sortedUnitsWithinTargetRange has no elements. No nearby units to evaluate?");
            return null;
        }

        newTarget = sortedUnitsWithinTargetRange[0];
        
        return newTarget;
    }
    
    private void OnDrawGizmos()
    {
        Vector3 _myPositionGizmos = new Vector3(transform.position.x, transform.position.y + (2 * sphereCastHeightModifier), transform.position.z);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_myPositionGizmos, _radiusOfSphereCast);
    }

    public void SetCombatClass(CombatClass combatClass)
    {
        _combatClass = combatClass;
    }
    
    protected Unit GetClosestEnemy()
    {
        List<Unit> sortedUnitsWithinTargetRange = GetSortedEnemiesInTargetRange();
        
        if (sortedUnitsWithinTargetRange.Count <= 0)
        {
            Debug.Log(
                "TargetingComponent.GetClosestEnemy: sortedUnitsWithinTargetRange has no entries. No nearby units to evaluate?");
            return null;
        }

        return EvaluateAndReturnNewTarget(sortedUnitsWithinTargetRange);
    }

    protected void SetTarget(Unit newTarget)
    {
        if (newTarget == _currentTarget)
        {
            return;
        }
        
        _currentTarget = newTarget;
        
        Debug.Log($"{gameObject.name} is Updating Target!");
        Debug.Log($"Target is {_currentTarget.name} with id {_currentTarget.GetInstanceID()}");
        
        
        _timeUntilNextTargetSwitch = 0f;
        
        //TODO: ?
        OnTargetUpdated?.Invoke(_currentTarget);
    }

    protected bool CanChangeTarget()
    {
        return _timeUntilNextTargetSwitch >= _targetSwitchDelay;
    }

    public Unit BT_GetCurrentTarget()
    {
        return _currentTarget;
    }

    public bool BT_HasTarget()
    {
        return _currentTarget != null;
    }
    
    public virtual void Tick()
    {
        if (_timeUntilNextTargetSwitch < _targetSwitchDelay)
        {
            _timeUntilNextTargetSwitch += Time.deltaTime;
        }

        if (_currentTarget != null && _currentTarget.isActiveAndEnabled == false)
        {
            _currentTarget = null;
        }
        
        if (!BT_HasTarget() || (CanChangeTarget() && !BT_IsCurrentTargetInAttackRange()))
        {
            SetTarget(GetClosestEnemy());
            return;
        }

        if (_currentTarget == null)
        {
            Debug.Log($"TargetingComponent.Tick: _currentTarget for {gameObject.name} is null despite attempts to set.");
        }
    }
    
}
