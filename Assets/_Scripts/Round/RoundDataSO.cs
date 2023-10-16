using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu (fileName = "Round ", menuName = "ScriptableObjects/Round/RoundData")]
public class RoundDataSO : SerializedScriptableObject
{
    [SerializeField] private int roundNumber;
    [SerializeField] private Dictionary<UnitDataSO, int> unitsToSpawn = new Dictionary<UnitDataSO, int>();
}
