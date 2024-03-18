using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Unit.Attributes.AttributeSet;
using _Scripts.Unit.GridBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using Component = _Scripts.Unit.GridBehaviour.Component;

public class HealthComponent : Component, ICombatStats
{
    [SerializeField] private int _currentHealth;
    private bool _dead;
    private StatMaxHealth _maxHealth;

    public override void Start()
    {
        _currentHealth = Mathf.RoundToInt(_maxHealth.Value);
    }

    public void TakeDamage(int damageValue)
    {
        if (_dead)
        {
            return;
        }
        
        Debug.Log($"{gameObject.name} taking damage: {damageValue}");
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
        Debug.Log($"{gameObject.name} has died.");
        gameObject.SetActive(false);
    }

    public void SetCharacterStats(List<CharacterStat> characterStats)
    {
        foreach (CharacterStat characterStat in characterStats)
        {
            if (characterStat is not StatMaxHealth statMaxHealth)
            {
                continue;
            }

            _maxHealth = statMaxHealth;
            break;
        }

        if (_maxHealth == null)
        {
            Debug.Log("HealthComponent.SetCharacterStats: _maxHealth not set.");
        }
    }
}
