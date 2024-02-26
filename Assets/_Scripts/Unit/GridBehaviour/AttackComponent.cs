using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    private int _attackSpeed;
    private int _attackDamage;
    private float _lastAttackTime;
    private Unit _target;  
    private HealthComponent _healthComponent;
    
    public bool CanAttack()
    {
        float currentTime = Time.realtimeSinceStartup;
        return currentTime - _attackSpeed <= _lastAttackTime;
    }

    public void AttackUnit(Unit target)
    {
        target.GetComponent<HealthComponent>().TakeDamage(_attackDamage);

        _lastAttackTime = Time.realtimeSinceStartup;
    }

    public void SetInitialDamage(int initialAttackDamage)
    {
        _attackDamage = initialAttackDamage;
    }

    public void SetInitialAttackSpeed(int initialAttackSpeed)
    {
        _attackSpeed = initialAttackSpeed;
    }
}
