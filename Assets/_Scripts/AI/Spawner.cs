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
    private GlobalBlackboard _test;
    private Variable _testUnits;
    [SerializeField] private Unit unitToSpawn;
    private Unit unitSpawned;
    [SerializeField] private Transform destination;
    private NavMeshAgent _unitNavMeshAgent;
    private float _timeBetweenSpawns = 0.7f;
    private float _timeElapsed;
    
    
    
    //debug
    private bool spawning = true;
    private List<GameObject> testing = new List<GameObject>();

    private void Start()
    {
        _test = FindObjectOfType<GlobalBlackboard>();
        _testUnits = _test.GetVariable("_unitsOnNavmesh");

        testing = _testUnits.value as List<GameObject>;
        
        
        
        
    }

    private void Update()
    {
        if (!spawning)
        {
            return;
        }
        
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            _unitNavMeshAgent.SetDestination(destination.position);
        }
        
        _timeElapsed += Time.deltaTime;
        if (_timeBetweenSpawns < _timeElapsed)
        {
            
            _timeElapsed = 0f;
            unitSpawned = Instantiate(unitToSpawn, transform.position, quaternion.identity);
            unitSpawned.SetFaction(1);
            //unitSpawned.AddComponent<UnitGridBehaviour>();
            unitSpawned.AddComponent<NavMeshAgent>();
            
            _unitNavMeshAgent = unitSpawned.GetComponent<NavMeshAgent>();
            _unitNavMeshAgent.enabled = false;
            _unitNavMeshAgent.enabled = true;
            _unitNavMeshAgent.SetDestination(destination.position);
            testing.Add(unitSpawned.gameObject);
            //Debug.Log(testing.Count);

        }
    }
}
