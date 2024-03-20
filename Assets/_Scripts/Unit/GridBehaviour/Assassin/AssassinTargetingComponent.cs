using System;
using System.Collections.Generic;
using UnityEngine;

public class AssassinTargetingComponent : TargetingComponent
{
    public event Action<List<string>, Type> OnValuesUpdated;
    
    //Should this be replaced by a generic function? TODO:
    //Also
    //Change this so I don't call AttackRange directly (protected field may be bad), and instead I
    //maybe call SetCharacterStats.base first and pass it what it might need first, then do my subclass
    //specific stuff
    public override void SetCharacterStats(List<CharacterStat> characterStats)
    {
        base.SetCharacterStats(characterStats);
    }

    private Unit GetNewEnemy()
    {
        List<Unit> sortedUnitsWithinTargetRange = GetSortedEnemiesInTargetRange();
        
        if (sortedUnitsWithinTargetRange.Count <= 0)
        {
            Debug.Log(
                "AssassinTargetingComponent.GetFurthestEnemyOfClassType: sortedUnitsWithinTargetRange has no elements. No nearby units to evaluate?");
            return null;
        }

        return EvaluateAndReturnNewTarget(sortedUnitsWithinTargetRange);
    }

    public override void Tick()
    {
        if (base._timeUntilNextTargetSwitch < base._targetSwitchDelay)
        {
            base._timeUntilNextTargetSwitch += Time.deltaTime;
        }

        if (base.BT_HasTarget())
        {
            if (base.CanChangeTarget() && !base.BT_IsCurrentTargetInAttackRange())
            {
                base.SetTarget(GetNewEnemy());
                return;
            }
        }
        
        if (!base.BT_HasTarget())
        {
            base.SetTarget(GetClosestEnemy());
            return;
        }
    }

    public bool BT_BehindTarget()
    {
        return transform.position.z < _currentTarget.transform.position.z;
    }
    
    //Evaluates list of SortedUnits by CombatClass(UnitCombatDataSO) specific targeting logic
    protected override Unit EvaluateAndReturnNewTarget(List<Unit> sortedUnitsWithinTargetRange)
    {
        Unit newTarget = null;
        
        //Find unit that matches preferred CombatClass on lowest Y world position
        foreach (Unit unit in sortedUnitsWithinTargetRange)
        {
            if (unit.GetComponentInChildren<AIController>().GetCombatClass() != CombatClass.Ranged)
            {
                continue;
            }

            if (newTarget == null)
            {
                newTarget = unit;
            } else
            {
                newTarget = unit.transform.position.z < newTarget.transform.position.z ? unit : newTarget;
                break;
            }
        }

        
        //Send event to brain to update target? Maybe in the SetTarget function in the base class?
        //UpdateBlackboard("_hasTarget", newTarget != null);

        //If we still don't have a target, choose the furthest away enemy regardless of CombatClass
        if (newTarget == null && sortedUnitsWithinTargetRange.Count > 0)
        {
            newTarget = sortedUnitsWithinTargetRange[sortedUnitsWithinTargetRange.Count - 1];
        }

        return newTarget;
    }

}