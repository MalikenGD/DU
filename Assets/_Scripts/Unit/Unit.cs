using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private int _faction;

    public void SetFaction(int faction)
    {
        _faction = faction;
    }

    public int GetFaction()
    {
        return _faction;
    }
}