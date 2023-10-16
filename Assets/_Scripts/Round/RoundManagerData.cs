using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "RoundManagerData", menuName = "ScriptableObjects/Round/RoundManagerData")]
public class RoundManagerData : ScriptableObject
{
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private List<RoundDataSO> rounds;
}
