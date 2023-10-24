using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    private Unit _target;
    private int _attackDamage;
    private HealthComponent _healthComponent;

    public bool AttackUnit(Unit target)
    {
        if (_target == null)
        {
            _target = target;
        }
        
        TryGetComponent<HealthComponent>(out HealthComponent enemyHealthComponent);

        if (enemyHealthComponent != null)
        {
            enemyHealthComponent.TakeDamage(_attackDamage);
            return true;
        }

        return false;
        
    }

    public void SetInitialDamage(int initialAttackDamage)
    {
        _attackDamage = initialAttackDamage;
    }
}
