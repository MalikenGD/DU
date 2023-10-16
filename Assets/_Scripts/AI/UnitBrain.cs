using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitBrain : Brain
{
    [SerializeField] private BehaviourTreeOwner behaviourTreeOwner;
    private Unit _unit;
    private Transform _target;

    private void Start()
    {
        behaviourTreeOwner.StartBehaviour();
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
