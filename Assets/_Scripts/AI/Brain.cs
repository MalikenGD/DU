using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Brain
{
    private NavMeshAgent _navMeshAgent;
    private BehaviourTreeOwner _behaviourTreeOwner;
    private Blackboard _blackboard;
    private Unit _controlledUnit;
    private readonly MovementComponent _unitMovementComponent;
    //private UnitGridBehaviour _unitGridBehaviour;

    private Unit _targetUnit;
    

    public List<Vector3> _cellPositions = new List<Vector3>(70);
    
    
    
    public Brain(Unit controlledUnit)
    {
        _controlledUnit = controlledUnit;

        _unitMovementComponent = _controlledUnit.gameObject.AddComponent<MovementComponent>();
        _navMeshAgent = _controlledUnit.AddComponent<NavMeshAgent>();
        //_unitGridBehaviour = _controlledUnit.AddComponent<UnitGridBehaviour>();
        _behaviourTreeOwner = _controlledUnit.AddComponent<BehaviourTreeOwner>();
        _blackboard = _controlledUnit.AddComponent<Blackboard>();

        _behaviourTreeOwner.blackboard = _blackboard;

        InitializeBlackboard();
    }

    private void InitializeBlackboard()
    {
        /*if (_targetUnit == null)
        {
            Debug.LogError("Brain.InitializeBlackboard: _targetUnit is Null.");
            return;
        }*/
        
        
        Variable test = _blackboard.AddVariable<Vector3>("_selfPosition", _controlledUnit.transform.position);
        
        test.BindProperty(typeof(Transform).GetMember("Position", BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault());
        Variable navMeshDestination = _blackboard.AddVariable("_navMeshDestination", _navMeshAgent.destination);
        _blackboard.AddVariable("_stoppingDistance", _navMeshAgent.stoppingDistance);
        //_blackboard.AddVariable("_targetPosition", _targetUnit.transform.position);
        //_blackboard.AddVariable("_target", _targetUnit);
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
