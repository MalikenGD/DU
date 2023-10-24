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
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private UnitGridBehaviour unitGridBehaviour; 
    private Unit _controlledUnit;
    private UnitCombatDataSO _unitCombatDataSO;

    
    //AIController should support multiple brains/BTs?
    private Brain _brain;

    public void Initialize(Unit unit, UnitCombatDataSO unitCombatDataSO)
    {
        SetControlledUnit(unit);
        SetUnitCombatData(unitCombatDataSO);
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        CreateBrain();
        navMeshAgent = _brain.navMeshAgent;

    }

    private void SetUnitCombatData(UnitCombatDataSO unitCombatDataSO)
    {
        _unitCombatDataSO = unitCombatDataSO;
    }

    public void ToggleNavMesh()
    {
        navMeshAgent.enabled = !navMeshAgent.enabled;
    }

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            navMeshAgent.SetDestination(Vector3.zero);
        }

        /*Debug.Log($"Brain Variable -selfPosition- is: {_brain.selfPosition.value}");
        Debug.Log($"Brain Variable -navMeshDestination- is: {_brain.navMeshDestination.value}");
        Debug.Log($"SelfPosition  is {_controlledUnit.transform.position}");
        Debug.Log($"Navmesh destination is {navMeshAgent.destination}");*/
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

    public void SetBehaviourTree(BehaviourTree behaviourTree)
    {
        _brain.SetBehaviourTree(behaviourTree);
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
