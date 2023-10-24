using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Brain
{
    private UnitCombatDataSO _unitCombatDataSO;
    internal NavMeshAgent navMeshAgent;
    private TargetingComponent _targetingComponent;
    private AttackComponent _attackComponent;
    private HealthComponent _healthComponent;
    private BehaviourTreeOwner _behaviourTreeOwner;
    private Blackboard _blackboard;
    private Unit _controlledUnit;
    private MovementComponent _unitMovementComponent;
    

    private Unit _targetUnit;

    public Variable selfPosition;
    public Variable navMeshDestination;
    public Variable target;
    
    
    public Brain(Unit controlledUnit, UnitCombatDataSO unitCombatDataSO)
    {
        _unitCombatDataSO = unitCombatDataSO;
        if (_unitCombatDataSO == null)
        {
            Debug.LogError("Brain.Brain(Constructor): _unitCombatDataSO is Null.");
            return;
        }
        _controlledUnit = controlledUnit;

        InitializeCombatData();
        InitializeBlackboard();

        _behaviourTreeOwner.StartBehaviour();
    }

    private void InitializeCombatData()
    {
        _unitMovementComponent = _controlledUnit.gameObject.AddComponent<MovementComponent>();
        navMeshAgent = _controlledUnit.gameObject.GetComponent<NavMeshAgent>();
        _targetingComponent = _controlledUnit.gameObject.AddComponent<TargetingComponent>();
        
        _attackComponent = _controlledUnit.gameObject.AddComponent<AttackComponent>();
        _attackComponent.SetInitialDamage(_unitCombatDataSO.GetInitialAttackDamage());
        
        _healthComponent = _controlledUnit.gameObject.AddComponent<HealthComponent>();
        _healthComponent.SetInitialHealth(_unitCombatDataSO.GetInitialHealth());
        
        _behaviourTreeOwner = _controlledUnit.AddComponent<BehaviourTreeOwner>();
        _blackboard = _controlledUnit.AddComponent<Blackboard>();
        
        _behaviourTreeOwner.blackboard = _blackboard;
        _behaviourTreeOwner.behaviour = _unitCombatDataSO.GetInitialBehaviourTree();
    }

    private void InitializeBlackboard()
    {
        
        selfPosition = _blackboard.AddVariable<Vector3>("_selfPosition");
        MemberInfo[] transformData = typeof(Transform).GetMember("position");
        BindProperty(selfPosition, transformData);
        
        navMeshDestination = _blackboard.AddVariable<Vector3>("_navMeshDestination");
        MemberInfo[] navMeshData = navMeshAgent.GetType().GetMember("destination");
        BindProperty(navMeshDestination, navMeshData);
        
        /*target = _blackboard.AddVariable<GameObject>("_target");
        MemberInfo[] targetData = _navMeshAgent.GetType().GetMember("destination");
        BindProperty(target, navMeshData);*/
        
        
        
        
        
        //test.BindProperty(memberInfos[0], _controlledUnit.gameObject);
        //test.BindProperty(typeof(Transform).GetMember("Position", BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault());
        //Variable navMeshDestination = _blackboard.AddVariable("_navMeshDestination", _navMeshAgent.destination);
        //_blackboard.AddVariable("_stoppingDistance", _navMeshAgent.stoppingDistance);
        //_blackboard.AddVariable("_targetPosition", _targetUnit.transform.position);
        //_blackboard.AddVariable("_target", _targetUnit);
    }

    private void BindProperty(Variable blackboardVariable, MemberInfo[] binding)
    {
        blackboardVariable.BindProperty(binding[0], _controlledUnit.gameObject);
    }
    
    internal void OnGameStateChanged(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.CombatPhase:
                _behaviourTreeOwner.StartBehaviour(); 
                break;
            case GameState.BuyPhase:
                _behaviourTreeOwner.StopBehaviour();
                break;
            case GameState.GameStart:
                break;
            default:
                Debug.LogError("Brain.OnGameStateChanged: State out of range.");
                break;
        }
    }
}
