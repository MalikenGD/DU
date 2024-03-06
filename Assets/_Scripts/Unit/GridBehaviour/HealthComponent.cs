using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private int _initialHealth;
    [SerializeField] private int _currentHealth;
    private bool _dead;

    private void Start()
    {
        _currentHealth = _initialHealth;
    }

    public void TakeDamage(int damageValue)
    {
        if (_dead)
        {
            return;
        }
        
        Debug.Log($"{transform.name} taking damage: {damageValue}");
        if (_currentHealth > damageValue)
        {
            _currentHealth -= damageValue;
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        _dead = true;
        //TODO:
        //Make sure any lists containing this unit you remove from and unsub to any events?
        Debug.Log("Death");
        gameObject.SetActive(false);
    }

    public void SetInitialHealth(int initialHealth)
    {
        _initialHealth = initialHealth;
    }
}
