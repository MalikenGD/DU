using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //TODO:
    //mesh/model?
    //name?
    //stats(Scriptable object?) - Atk range, damage, health

    public GridPosition _gridPosition;
    private bool _isDamageable = true;

    public enum Faction
    {
        Friendly,
        Enemy
    }

    private Faction _factionType;

    private Unit target;

    private GridObject _gridObject;

    private void Start()
    {
        //TODO: Refactor to constructor
        SetGridObject(GridManager.Instance.GetGridObjectAtPosition(GetComponent<Transform>().position));
        _gridPosition = GridManager.Instance.ConvertFromWorldPositionToGridPosition(transform.position);
        GridManager.Instance.SetUnitAtGridPosition(_gridPosition, this);
    }


    //TODO: Implement this constructor instead of doing it on Start()
    public Unit(Faction faction, bool isDamageable, GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
        _factionType = faction;
        _isDamageable = isDamageable;
    }


    public GridObject GetGridObject()
    {
        return _gridObject;
    }

    public void SetGridObject(GridObject gridobjectToSet)
    {
        _gridObject = gridobjectToSet;
    }

    public Faction GetFaction()
    {
        return _factionType;
    }

    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }
}
