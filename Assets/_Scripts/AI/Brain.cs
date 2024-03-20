using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Scripts.Unit.GridBehaviour;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Component = _Scripts.Unit.GridBehaviour.Component;

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
    private MovementComponent _movementComponent;
    

    private Unit _targetUnit;

    //tickables sorted by Priority
    private List<ITickable> _tickables = new List<ITickable>(); 
    private List<Component> _components = new List<Component>();
    
    //private List<Unit> _
    private List<CharacterStat> _characterStats;
    
    
    public Brain(Unit controlledUnit, UnitCombatDataSO unitCombatDataSO)
    {
        _components = controlledUnit.GetComponents<Component>().ToList();
        foreach (Component component in _components)
        {
            if (component is ITickable tickable)
            {
                _tickables.Add(tickable);
            }
        }
        
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
        _characterStats = _unitCombatDataSO.GetCharacterStats();

        foreach (Component component in _components)
        {
            if (component is ICombatStats combatStats)
            {
                combatStats.SetCharacterStats(_characterStats);
            }
        }

        //TODO: Gotta be a better way to do this, as there should be plenty of these events to subscribe to.
        //_targetingComponent.OnTargetUpdated += OnTargetUpdated;

        /*switch (_unitCombatDataSO.GetCombatClass())
        {
            case CombatClass.Assassin:
                _attackComponent = _controlledUnit.AddComponent<AssassinAttackComponent>();
                _targetingComponent = _controlledUnit.AddComponent<AssassinTargetingComponent>();
                _movementComponent = _controlledUnit.AddComponent<AssassinMovementComponent>();
                break;
            default:
                break;
        }*/
    }
    
    /*private void OnTargetUpdated(Unit newTarget)
    {
        _movementComponent.UpdateGoalPosition(newTarget.transform.position);
    }*/
    
    private void InitializeBlackboard()
    {
        
        _behaviourTreeOwner = _controlledUnit.AddComponent<BehaviourTreeOwner>();
        _blackboard = _controlledUnit.AddComponent<Blackboard>();
        
        _behaviourTreeOwner.behaviour = _unitCombatDataSO.GetInitialBehaviourTree();


        /*switch (_unitCombatDataSO.GetCombatClass())
        {
            case CombatClass.Assassin:
                //_blackboard.AddVariable(_attackComponent.GetType().ToString());
                _blackboard.SetVariableValue("_attackComponent", _attackComponent);
                _blackboard.SetVariableValue("_targetingComponent", _targetingComponent);
                _blackboard.SetVariableValue("_movementComponent", _movementComponent);
                break;
            default:
                break;
        }*/



        /*selfPosition = _blackboard.AddVariable<Vector3>("_selfPosition");
        MemberInfo[] transformData = typeof(Transform).GetMember("position");
        BindProperty(selfPosition, transformData);

        navMeshDestination = _blackboard.AddVariable<Vector3>("_navMeshDestination");
        MemberInfo[] navMeshData = navMeshAgent.GetType().GetMember("destination");
        BindProperty(navMeshDestination, navMeshData);*/

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

    public void Update()
    {
        //foreach component, tick
        //so...


        TickObjects();
        
        //targeting component has target
        //if my BT expects a target, I need to get that value and update BT
        //but if it doesn't, then I don't care?
        
        
        //UpdateBlackboard();

        //but they need to be in order, so... priority?
        
        
        /*switch (_unitCombatDataSO.GetCombatClass())
        {
            case CombatClass.Assassin:
                
                _blackboard.SetVariableValue("_hasTarget", _targetingComponent.GetCurrentTarget() != null);
                _blackboard.SetVariableValue("_attackOnCooldown", _targetingComponent.GetCurrentTarget() != null);
                _blackboard.SetVariableValue("_inRangeOfTarget", _targetingComponent.GetCurrentTarget() != null);
                _blackboard.SetVariableValue("_behindTarget", _targetingComponent.GetCurrentTarget() != null);
                break;
        }*/
    }

    private void TickObjects()
    {
        foreach (ITickable tickables in _tickables)
        {
            tickables.Tick();
        }
    }

    private void BindProperty(Variable blackboardVariable, MemberInfo[] binding)
    {
        blackboardVariable.BindProperty(binding[0], _controlledUnit.gameObject);
    }
    
    public void OnGameStateChanged(GameState newGameState)
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

    

    private void UpdateBlackboard<T>(string variableName, T value)
    {
        _behaviourTreeOwner.blackboard.SetVariableValue(variableName, value);
    }
}
