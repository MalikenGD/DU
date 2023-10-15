using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerDataSO startingPlayerData;
    private int _goldBalance;
    private int _remainingLives;


    private void Start()
    {
        bool isPlayerDataNull = startingPlayerData == null;

        if (isPlayerDataNull)
        {
            Debug.LogError("Player.Start: StartingPlayerData is null");
        }
        
        _goldBalance = startingPlayerData.GetStartingGold();
        _remainingLives = startingPlayerData.GetStartingLives();
    }
}
