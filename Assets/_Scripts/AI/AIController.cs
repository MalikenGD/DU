using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class AIController : Controller
{
    private Unit _controlledUnit;
    private UnitGridBehaviour _unitGridBehaviour; 
    private UnitCombatDataSO _unitCombatDataSO;
    private TargetingComponent _targetingComponent;
    
    //AIController should support multiple brains/BTs?
    private Brain _brain;

    public void Initialize(Unit unit, UnitCombatDataSO unitCombatDataSO)
    {
        SetControlledUnit(unit);
        SetUnitCombatData(unitCombatDataSO);
        //NavMeshAgent navMeshAgent = GetComponentInParent<NavMeshAgent>();
        CreateBrain();
        //navMeshAgent = _brain.navMeshAgent;

    }

    private void SetUnitCombatData(UnitCombatDataSO unitCombatDataSO)
    {
        _unitCombatDataSO = unitCombatDataSO;
    }

    private void CreateBrain()
    {
        _brain = new Brain(_controlledUnit, _unitCombatDataSO);
        World.Instance.OnGameStateChanged += _brain.OnGameStateChanged;
    }

    private void DestroyBrain()
    {
        World.Instance.OnGameStateChanged -= _brain.OnGameStateChanged;
        _brain = null;
    }
    

    private void SetControlledUnit(Unit unit)
    {
        _controlledUnit = unit;
    }
    
    public override void HandleInputs()
    {
        //Take input from Brain
    }

    private void Update()
    {
        if (!_targetingComponent.IsCurrentTargetInRange())
        {
            List<Unit> sortedUnitsWithinTargetRange = _targetingComponent.GetSortedEnemiesInTargetRange();
            
            if (sortedUnitsWithinTargetRange.Count <= 0)
            {
                Debug.Log(
                    "AIController.Update: enemyUnitsInTargetRange is null. No nearby units to evaluate?");
                return;
            }
            
            Unit newTarget = _brain.EvaluateAndReturnNewTarget(sortedUnitsWithinTargetRange);

            if (newTarget == null)
            {
                Debug.Log("AIController.Update: newTarget is Null.");
                return;
            }
            _targetingComponent.SetTarget(newTarget);
            
        }
    }

    public CombatClass GetCombatClass()
    {
        return _unitCombatDataSO.GetCombatClass();
    }
}
