using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    [SerializeField] private float heightModifier = 2.5f;
    private Unit _currentTarget = null;
    private float _radiusOfSphereCast;
    private Vector3 _myPositionGizmos;
    private CombatClass _combatClass;
    private int _attackRange;
    private float _attackRangeGraceDistance; // Wiggle room, added to attack range
    private int _targetingRange;

    public void ChooseTarget()
    {
        //Throw to brain? Inherited TargetingComponent? 
        
        
        
        switch (_combatClass)
        {
            case(CombatClass.Assassin):
                //if assassin, choose furthest target
                //spherecast
                //sort by distance
                //if in targetrange / 2 (or maybe just targeting range?)
                //...but it's quite large for assassins
                //jump

                break;
            case(CombatClass.MageKiller):
                //if magekiller, choose any mana user
                break;
            default:
                //if melee/ranged, choose nearest target
                break;
        }
    }

    public Unit EvaluateTarget()
    {
        Unit newTarget = null;
        
        if (_currentTarget != null)
        {
            if (Vector3.Distance(_currentTarget.transform.position, transform.position) <= (_attackRange + _attackRangeGraceDistance))
            {
                //Maybe add wiggle room so you can still attack despite moving +0.5f?
                return _currentTarget;
            }
        }
        
        List<GameObject> enemyUnitsInTargetRange = GetSortedEnemiesInTargetRange();
        if (enemyUnitsInTargetRange == null)
        {
            Debug.Log(
                "TargetingComponent.EvaluateTarget: enemyUnitsInTargetRange is null. No nearby units to evaluate?");
            return null;
        }

        foreach (GameObject unit in enemyUnitsInTargetRange)
        {
            //If we've set a new target during a previous enumeration
            if (newTarget != null)
            {
                break;
            }
            
            switch (unit.GetComponent<TargetingComponent>()._combatClass)
            {
                case CombatClass.Ranged:
                    newTarget = unit.GetComponent<Unit>();
                    break;
            }
        }
        
        //If No nearby unit was suitable
        if (newTarget == null)
        {
            //Set nearest unit as Target regardless of CombatClass
            return enemyUnitsInTargetRange[0].GetComponent<Unit>();
        }
        
        
            
        //maybe this throws the list to a specialized brain (assassin brain) that takes the list
        //and spits out the target it wants, and if it returns not null, then it exits this method
        //and if it returns null,we just choose enemy closest?
        
        
        

        //TODO:
        //Pretty sure all this and below is irrelevant now 
        foreach (GameObject unit in GetSortedEnemiesInTargetRange())
        {
            int unitFaction = unit.GetComponent<Unit>().GetFaction();
            int myFaction = GetComponent<Unit>().GetFaction();
            if (myFaction != unitFaction)
            {
                if (unit.activeSelf)
                {
                    return unit.GetComponent<Unit>();
                }
            }
        }

        return null;


        /*if (currentTarget != null)
        {
            if (!currentTarget.gameObject.activeSelf)
            {
                return null;
            }
        }

        GameObject closestUnit = null;

        foreach (GameObject unit in SphereCast())
        {
            if (!unit.gameObject.activeSelf)
            {

                continue;
            }
            Debug.Log("2");

            int myFaction = GetComponent<Unit>().GetFaction();
            if (unit.GetComponent<Unit>().GetFaction() != myFaction)
            {
                closestUnit = unit;
                break;
            }
        }

        if (closestUnit == null)
        {
            Debug.LogError("TargetingComponent.EvaluateTarget: ClosestUnit is Null.");
            return null;
        }

        return closestUnit.GetComponent<Unit>();*/
    }

    //Spherecast from above to below, storing units in a sorted list, excluding self.
    //Sorts by range to self
    private List<GameObject> GetSortedEnemiesInTargetRange()
    {
        _radiusOfSphereCast = _targetingRange;
        List<GameObject> unitsHit = new List<GameObject>();
        Vector3 myPosition = transform.position;
        Vector3 sphereCastStartPosition = new Vector3(myPosition.x, myPosition.y + (2 * heightModifier), myPosition.z);
        
        int myFaction = GetComponent<Unit>().GetFaction();
        
        foreach (RaycastHit raycastHit in Physics.SphereCastAll(sphereCastStartPosition, _radiusOfSphereCast, Vector3.down))
        {
            int unitFaction;
            
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
                
            unitsHit.Add(raycastHit.collider.gameObject);
        }
        
        // Sort list by Range to self
        if (unitsHit.Count > 0)
        {
            unitsHit = unitsHit.OrderBy(go => Vector3.Distance(go.transform.position, myPosition)).ToList();
        }
        
        return unitsHit.Count > 0 ? unitsHit : null;
    }
    
    void OnDrawGizmos()
    {
        _myPositionGizmos = new Vector3(transform.position.x, transform.position.y + (2 * heightModifier), transform.position.z);
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
}
