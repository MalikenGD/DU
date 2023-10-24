using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu (fileName = "_CombatData", menuName = "ScriptableObjects/Unit/UnitCombatData")]
public class UnitCombatDataSO : ScriptableObject
{
    [SerializeField] private int initialHealth;
    [SerializeField] private int initialAttackDamage;
    [SerializeField] private int initialAttackSpeed;

    public int GetInitialHealth()
    {
        return initialHealth;
    }
    public int GetInitialAttackDamage()
    {
        return initialAttackDamage;
    }
    public int GetInitialAttackSpeed()
    {
        return initialAttackSpeed;
    }
    
    

}
