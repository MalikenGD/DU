using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerDataSO _startingPlayerData;
    [SerializeField] private PlayerController _playerController;
    private int _goldBalance;
    private int _remainingLives;


    private void Start()
    {
        _goldBalance = _startingPlayerData.GetStartingGold();
        _remainingLives = _startingPlayerData.GetStartingLives();
    }
}
