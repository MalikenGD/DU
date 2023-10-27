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
    private UnitGridBehaviour _unitGridBehaviour; 
    private Unit _controlledUnit;
    private UnitCombatDataSO _unitCombatDataSO;

    
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
}
