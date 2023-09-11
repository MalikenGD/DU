using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "CardData", menuName = "ScriptableObjects/Card/CardData")]
public class CardDataSO : ScriptableObject
{
    [SerializeField] private GameObject cardUIPrefab;

    public GameObject GetCardUIPrefab()
    {
        return cardUIPrefab;
    }
}
