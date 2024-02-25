using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSet : MonoBehaviour
{
    [SerializeField] private UnitCombatDataSO unitCombatData;
    
    private List<CharacterStat> stats;

    private void Start()
    {
        if (unitCombatData != null)
        {
            stats = new List<CharacterStat>();
            
        }
        stats = new List<CharacterStat>();
    }
}
