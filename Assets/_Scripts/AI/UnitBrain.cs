using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class UnitBrain : Brain
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private BehaviourTreeOwner behaviourTreeOwner;
    private Unit _unit;
    public Unit _target;
    public Vector3 _targetPosition;

    public List<Vector3> _cellPositions = new List<Vector3>(70);

    private void Awake()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _cellPositions.Add(new Vector3(i + 51,0,j + 21));
            }
        }
        _targetPosition = new Vector3(54f, 0f, 30f);
    }

    private void Start()
    {
        
        navMeshAgent.SetDestination(transform.position);
        behaviourTreeOwner.StartBehaviour();
    }

    private void Update()
    {
        UnityEngine.Debug.Log(navMeshAgent.destination);
    }

    private void OnEnable()
    {
        World.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        World.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.CombatPhase:
                behaviourTreeOwner.StartBehaviour();
                break;
            case GameState.BuyPhase:
                behaviourTreeOwner.StopBehaviour();
                break;
            case GameState.GameStart:
                break;
            default:
                Debug.LogError("UnitBrain.OnGameStateChanged: State out of range.");
                break;
        }
    }

    public void SetBehaviourTree(BehaviourTree behaviourTree)
    {
        behaviourTreeOwner.behaviour = behaviourTree;
    }
}
