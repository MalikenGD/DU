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
    internal NavMeshAgent navMeshAgent;
    private BehaviourTreeOwner _behaviourTreeOwner;
    private Blackboard _blackboard;
    private Unit _controlledUnit;
    private readonly MovementComponent _unitMovementComponent;
    

    private Unit _targetUnit;

    public Variable selfPosition;
    public Variable navMeshDestination;
    public Variable target;
    
    
    public Brain(Unit controlledUnit)
    {
        _controlledUnit = controlledUnit;

        _unitMovementComponent = _controlledUnit.gameObject.AddComponent<MovementComponent>();
        navMeshAgent = _controlledUnit.gameObject.GetComponent<NavMeshAgent>();
        
        _behaviourTreeOwner = _controlledUnit.AddComponent<BehaviourTreeOwner>();
        _blackboard = _controlledUnit.AddComponent<Blackboard>();
        
        _behaviourTreeOwner.blackboard = _blackboard;

        InitializeBlackboard();
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
    
    /*private void Awake()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _cellPositions.Add(new Vector3(i + 51,0,j + 21));
            }
        }
    }*/

    private void MoveTo(Vector3 newPosition)
    {
        _unitMovementComponent.MoveTo(newPosition);
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

    public void SetBehaviourTree(BehaviourTree behaviourTree)
    {
        _behaviourTreeOwner.behaviour = behaviourTree;
    }
}
