using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private int _initialHealth;
    [SerializeField] private int _currentHealth;

    private void Start()
    {
        _currentHealth = _initialHealth;
    }

    public void TakeDamage(int damageValue)
    {
        Debug.Log($"Taking damage: {damageValue}");
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
        Debug.Log("Death");
        gameObject.SetActive(false);
    }

    public void SetInitialHealth(int initialHealth)
    {
        _initialHealth = initialHealth;
    }
}
