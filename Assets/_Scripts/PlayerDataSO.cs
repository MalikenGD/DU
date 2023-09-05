using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "PlayerData", menuName = "ScriptableObjects/Player/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] private int startingGold;
    [SerializeField] private int startingLives;

    public int GetStartingGold()
    {
        return startingGold;
    }

    public int GetStartingLives()
    {
        return startingLives;
    }
}