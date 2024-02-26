using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AIController : Controller
{
    
    private Unit _controlledUnit;
    private UnitGridBehaviour _unitGridBehaviour; 
    private UnitCombatDataSO _unitCombatDataSO;
    private TargetingComponent _targetingComponent;
    private AttackComponent _attackComponent;
    private HealthComponent _healthComponent;
    
    //AIController should support multiple brains/BTs?
    private Brain _brain;


    public void Initialize(Unit unit, UnitCombatDataSO unitCombatDataSO)
    {
        SetControlledUnit(unit);
        SetUnitCombatData(unitCombatDataSO);
        InitializeCombatData();
        //NavMeshAgent navMeshAgent = GetComponentInParent<NavMeshAgent>();
        CreateBrain();
        //navMeshAgent = _brain.navMeshAgent;

    }
    
    private void InitializeCombatData()
    {
        /*_unitMovementComponent = _controlledUnit.gameObject.AddComponent<MovementComponent>();
        navMeshAgent = _controlledUnit.gameObject.GetComponent<NavMeshAgent>();
        */
        _targetingComponent = _controlledUnit.gameObject.AddComponent<TargetingComponent>();
        _targetingComponent.SetCombatClass(_unitCombatDataSO.GetCombatClass());
        _targetingComponent.SetAttackRange(_unitCombatDataSO.GetInitialAttackRange());
        _targetingComponent.SetTargetingRange(_unitCombatDataSO.GetTargetingRange());
        
        _attackComponent = _controlledUnit.gameObject.AddComponent<AttackComponent>();
        _attackComponent.SetInitialDamage(_unitCombatDataSO.GetInitialAttackDamage());
        _attackComponent.SetInitialAttackSpeed(_unitCombatDataSO.GetInitialAttackSpeed());
        
        _healthComponent = _controlledUnit.gameObject.AddComponent<HealthComponent>();
        _healthComponent.SetInitialHealth(_unitCombatDataSO.GetInitialHealth());
        
        
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

    public void Attack()
    {
        //Attempt to Attack
        if (_targetingComponent.IsCurrentTargetInRange() && _attackComponent.CanAttack())
        {
            _attackComponent.AttackUnit(_targetingComponent.GetCurrentTarget());
        }
    }

    public void Move()
    {
        Debug.Log("MOVE");
        //Movement Component
    }

    public void Chase()
    {
        Debug.Log("CHASE");
        //Movement Component
    }

    public void Leap()
    {
        Debug.Log("Leap");
        //Movement Component
    }

    private void Update()
    {
        //Check targeting
        if (!_targetingComponent.IsCurrentTargetInRange() && _targetingComponent.CanChangeTarget())
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
