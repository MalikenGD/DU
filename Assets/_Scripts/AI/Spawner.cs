using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Spawner : MonoBehaviour
{
    //Spawner should ideally hold RoundData to determine units/number to spawn.
    private GlobalBlackboard _globalBlackboard;
    private Variable _blackboardUnits;
    [SerializeField] private Unit unitToSpawn;
    [SerializeField] private UnitCombatDataSO unitCombatDataSO;
    [SerializeField] private AIController aiControllerPrefab;
    private Unit unitSpawned;
    [SerializeField] private Transform destination;
    private NavMeshAgent _unitNavMeshAgent;
    private MovementComponent _movementComponent;
    private float _timeBetweenSpawns = 0.7f;
    private float _timeElapsed;
    
    
    
    //debug
    private bool spawning = true;
    private List<GameObject> _Units = new List<GameObject>();
    private TargetingComponent _targetingComponent;

    private void Start()
    {
        _globalBlackboard = FindObjectOfType<GlobalBlackboard>();
        _blackboardUnits = _globalBlackboard.GetVariable("_unitsOnNavmesh");

        _Units = _blackboardUnits.value as List<GameObject>;
        
        
        
    }

    private void Update()
    {
        if (!spawning)
        {
            return;
        }
        
        _timeElapsed += Time.deltaTime;
        if (_timeBetweenSpawns < _timeElapsed)
        {
            
            _timeElapsed = 0f;
            SpawnUnit();
            unitSpawned.SetFaction(1);
            


            _unitNavMeshAgent = unitSpawned.GetComponent<NavMeshAgent>();
            _movementComponent = unitSpawned.GetComponent<MovementComponent>();
            _unitNavMeshAgent.enabled = false;
            _unitNavMeshAgent.enabled = true;
            _movementComponent.MoveTo(destination.position);
            _Units.Add(unitSpawned.gameObject);
            //Debug.Log(testing.Count);

        }
    }

    private void SpawnUnit()
    {
        unitSpawned = World.Instance.BuildUnit(unitToSpawn.gameObject);
        unitSpawned.transform.position = transform.position;
        unitSpawned.gameObject.AddComponent<NavMeshAgent>();
        
        
        AIController aiController = Instantiate(aiControllerPrefab, transform.parent, true);
        aiController.Initialize(unitSpawned, unitCombatDataSO);
        
        aiController.transform.parent = unitSpawned.transform;
}
}
