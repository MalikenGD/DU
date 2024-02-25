using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.Serialization;

public enum CombatClass
{
    Melee,
    Ranged,
    Assassin,
    MageKiller
}

[CreateAssetMenu (fileName = "_CombatData", menuName = "ScriptableObjects/Unit/UnitCombatData")]
public class UnitCombatDataSO : ScriptableObject
{
    [SerializeField] private BehaviourTree behaviourTree;
    [SerializeField] private CombatClass combatClass;
    [SerializeField] private List<CharacterStat> characterStats;
    
    [Header("Initial stat values.")]
    [SerializeField] private CharacterStat initialHealth;
    [SerializeField] private CharacterStat initialAttackDamage;
    [SerializeField] private CharacterStat initialAttackSpeed;
    [Tooltip("Range can attack an enemy. See SensorRange for targeting range.")]
    [SerializeField] private CharacterStat initialAttackRange;
    
    [Space]
    [Range(7,15)] [Tooltip("Range can detect enemies and set Target.")]
    [SerializeField] private int targetingRange;

    public BehaviourTree GetInitialBehaviourTree()
    {
        return behaviourTree;
    }
    
    public int GetInitialHealth()
    {
        return (int)initialHealth.Value;
    }
    public int GetInitialAttackDamage()
    {
        return (int)initialAttackDamage.Value;
    }
    public int GetInitialAttackSpeed()
    {
        return (int)initialAttackSpeed.Value;
    }

    public int GetInitialAttackRange()
    {
        return (int) initialAttackRange.Value;
    }

    public CombatClass GetCombatClass()
    {
        return combatClass;
    }


    public int GetTargetingRange()
    {
        return targetingRange;
    }
}
