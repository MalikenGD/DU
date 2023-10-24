using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/Unit/UnitData")]
public class UnitDataSO : ScriptableObject
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private UnitCombatDataSO unitCombatDataSO;
    [SerializeField] private string unitName;
    [SerializeField] private Sprite spriteForShopBackground;
    [SerializeField] private int unitGoldCost;

    public GameObject GetUnitPrefab()
    {
        return unitPrefab;
    }

    /*public Brain GetBrainPrefab()
    {
        return unitBrainprefab;
    }*/
    

    public string GetUnitName()
    {
        return unitName;
    }

    public Sprite GetUnitSpriteForShopBackground()
    {
        return spriteForShopBackground;
    }

    public int GetUnitGoldCost()
    {
        return unitGoldCost;
    }

    public UnitCombatDataSO GetUnitCombatData()
    {
        return unitCombatDataSO;
    }
}
