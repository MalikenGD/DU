using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Unit.Attributes.AttributeSet;
using _Scripts.Unit.GridBehaviour;
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

    public override void Tick()
    {
        base.Tick();
        
        if (!base.BT_IsCurrentTargetInAttackRange() && base.CanChangeTarget())
        {
            List<Unit> sortedUnitsWithinTargetRange = base.GetSortedEnemiesInTargetRange();
            
            if (sortedUnitsWithinTargetRange.Count <= 0)
            {
                Debug.Log(
                    "AssassinTargetingComponent.Tick: enemyUnitsInTargetRange is null. No nearby units to evaluate?");
                return;
            }
            
            Unit newTarget = EvaluateAndReturnNewTarget(sortedUnitsWithinTargetRange);

            if (newTarget == null)
            {
                Debug.Log($"AssassinTargetingComponent.Tick: newTarget is Null, {gameObject.name} failed to set new Target.");
                return;
            }
            
            SetTarget(newTarget);
        }
        else
        {
            if (base.BT_IsCurrentTargetInAttackRange() && BT_BehindTarget())
            {
                //TODO: Fire event: CanAttack (and send target)
                //Maybe make it so as components are added, they subscribe or brain subscribes?
            }
        }

        UpdateBTValues();
    }

    private void UpdateBTValues()
    {
        //OnValuesUpdated?.Invoke();
    }

    public bool BT_BehindTarget()
    {
        return transform.position.y < _currentTarget.transform.position.y;
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
                newTarget = unit.transform.position.y < newTarget.transform.position.y ? unit : newTarget;
                break;
            }
        }

        
        //Send event to brain to update target? Maybe in the SetTarget function in the base class?
        //UpdateBlackboard("_hasTarget", newTarget != null);

        return newTarget;
    }

}