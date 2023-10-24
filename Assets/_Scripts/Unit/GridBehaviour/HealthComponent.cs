using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private int _initialHealth;
    private int _currentHealth;
    public void TakeDamage(int damageValue)
    {
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
        //
    }

    public void SetInitialHealth(int initialHealth)
    {
        _initialHealth = initialHealth;
    }
}
