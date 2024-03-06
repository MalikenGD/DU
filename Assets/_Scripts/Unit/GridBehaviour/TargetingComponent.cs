using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Unit.GridBehaviour;
using Unity.VisualScripting;
using UnityEngine;

public class TargetingComponent : MonoBehaviour, IComponent
{
    [SerializeField] private float sphereCastHeightModifier = 2.5f;
    protected Unit _currentTarget = null;
    private bool _currentTargetOutOfRange;
    private float _radiusOfSphereCast;
    private CombatClass _combatClass;
    private int _attackRange;
    private float _attackRangeWiggleRoom; // Wiggle room, added to attack range
    private int _targetingRange;
    private const int TARGET_SWITCH_DELAY = 1;
    private float _timeUntilNextTargetSwitch = 0f;

    private void Update()
    {
        /*if (_timeUntilNextTargetSwitch < 1f)
        {
            _timeUntilNextTargetSwitch += Time.deltaTime;
        }*/
    }

    

    public bool IsCurrentTargetInRange()
    {
        //Has target died or otherwise been disabled
        if (_currentTarget == null || _currentTarget.gameObject.activeInHierarchy == false)
        {
            return false;
        }
        
        return Vector3.Distance
               (_currentTarget.transform.position, transform.position) <= (_attackRange + _attackRangeWiggleRoom);
    }

    //Spherecast from above to below, storing units in a sorted list, excluding self.
    //Sorts by range to self
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
            // Is other unit active (and parents active) in scene
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

    public void SetAttackRange(int attackRange)
    {
        _attackRange = attackRange;
    }

    public void SetTargetingRange(int targetingRange)
    {
        _targetingRange = targetingRange;
    }

    public void SetTarget(Unit newTarget)
    {
        _currentTarget = newTarget;
        _timeUntilNextTargetSwitch = 0f;
    }

    public bool CanChangeTarget()
    {
        return _timeUntilNextTargetSwitch >= TARGET_SWITCH_DELAY;
    }

    public Unit GetCurrentTarget()
    {
        return _currentTarget;
    }
    
    public void Tick()
    {
        if (_timeUntilNextTargetSwitch < 1f)
        {
            _timeUntilNextTargetSwitch += Time.deltaTime;
        }
    }
    
}
