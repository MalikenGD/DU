using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using Component = _Scripts.Unit.GridBehaviour.Component;

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
    
    [SerializeReference]
    private List<CharacterStat> characterStats;

    public BehaviourTree GetInitialBehaviourTree()
    {
        return behaviourTree;
    }

    public List<CharacterStat> GetCharacterStats()
    {
        return characterStats;
    }

    public CombatClass GetCombatClass()
    {
        return combatClass;
    }
}
