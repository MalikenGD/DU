using System.Collections;
using System.Collections.Generic;
using _Scripts.Unit.Attributes.AttributeSet;
using _Scripts.Unit.GridBehaviour;
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
        //_lastAttackTime = Mathf.Infinity;
        base.Start();
    }

    public bool BT_AttackReady()
    {
        Debug.Log($"Checking for attack ready. current time from startup is {Time.realtimeSinceStartup}, last attack time was {_lastAttackTime}, and my atkspd is {_attackSpeed.Value}. ");
        float currentTime = Time.realtimeSinceStartup;
        return currentTime >= _lastAttackTime + _attackSpeed.Value;
    }

    public void BT_AttackUnit(Unit target)
    {
        Debug.Log($"{gameObject.name} with id {gameObject.GetInstanceID()} is attacking {target.gameObject.name} with id {gameObject.GetInstanceID()}");
        target.TryGetComponent<HealthComponent>(out HealthComponent healthComponent);
        if (healthComponent != null)
        {
            Debug.Log($"Dealing {_attackDamage.Value} damage.");
            healthComponent.TakeDamage(Mathf.RoundToInt(_attackDamage.Value));
            _lastAttackTime = Time.realtimeSinceStartup;
        }
        else
        {
            Debug.Log($"AttackComponent.BT_AttackUnit: target ({target.gameObject.name}) has no healthComponent or it is null.");
        }
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
