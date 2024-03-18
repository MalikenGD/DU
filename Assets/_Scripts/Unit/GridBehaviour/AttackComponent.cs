using System.Collections;
using System.Collections.Generic;
using _Scripts.Unit.Attributes.AttributeSet;
using _Scripts.Unit.GridBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using Component = _Scripts.Unit.GridBehaviour.Component;

public class AttackComponent : Component, ITickable, ICombatStats
{
    private StatAttackSpeed _attackSpeed;
    private StatAttackDamage _attackDamage;
    private float _lastAttackTime;
    private HealthComponent _healthComponent;


    public override void Start()
    {
        priority = Priority.Late;
        base.Start();
    }

    public bool BT_AttackReady()
    {
        float currentTime = Time.realtimeSinceStartup;
        return currentTime - _attackSpeed.Value <= _lastAttackTime;
    }

    public void AttackUnit(Unit target)
    {
        target.TryGetComponent<HealthComponent>(out HealthComponent unitHealthComponent);
        if (unitHealthComponent != null)
        {
            unitHealthComponent.TakeDamage(Mathf.RoundToInt(_attackDamage.Value));
        }
        else
        {
            Debug.Log("AttackComponent.AttackUnit: target's HealthComponent is null/has no HealthComponent.");
        }

        _lastAttackTime = Time.realtimeSinceStartup;
    }

    public void Tick()
    {
        if (BT_AttackReady())
        {
            //fire event to Brain/BT to set AttackReady? TODO:
        }
    }

    public void SetCharacterStats(List<CharacterStat> characterStats)
    {
        foreach (CharacterStat characterStat in characterStats)
        {
            if (_attackDamage != null && _attackSpeed != null)
            {
                break;
            }
            switch (characterStat)
            {
                case StatAttackDamage statAttackDamage:
                    _attackDamage = statAttackDamage;
                    break;
                case StatAttackSpeed statAttackSpeed:
                    _attackSpeed = statAttackSpeed;
                    break;
                default:
                    break;
            }
        }

        if (_attackDamage == null)
        {
            Debug.Log("AttackComponent.SetCharacterStats: _attackDamage not set.");
        }

        if (_attackSpeed == null)
        {
            Debug.Log("AttackComponent.SetCharacterStats: _attackSpeed not set.");
        }
    }
}
