using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CharacterStat
{
    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    protected bool isDirty = true;

    public float BaseValue;

    protected float _lastBaseValue = float.MinValue;
    protected float _value;
    
    public virtual float Value
    {
        get
        {
            if (isDirty || BaseValue != _lastBaseValue)
            {
                _lastBaseValue = BaseValue;
                _value = CalculateValue();
                isDirty = false;
            }

            return _value;
        }
    }

    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public CharacterStat(float value) : this()
    {
        BaseValue = value;
    }

    public virtual void AddModifier(StatModifier modifier)
    {
        isDirty = true;
        statModifiers.Add(modifier);
        statModifiers.Sort(CompareModifierOrder);
    }

    public virtual bool RemoveModifier(StatModifier modifier)
    {
        if (statModifiers.Remove(modifier))
        {
            isDirty = true;
            return true;
        }

        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool modifiersRemoved = false;
        
        for (int i = statModifiers.Count; i > 0; i--)
        {
            if (statModifiers[i]._modifierSource == source)
            {
                isDirty = true;
                modifiersRemoved = true;
                statModifiers.RemoveAt(i);
            }
        }

        return modifiersRemoved;
    }
    
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a._modifierOrder < b._modifierOrder)
        {
            return -1;
        } 
        if (a._modifierOrder > b._modifierOrder)
        {
            return 1;
        }

        return 0; // if a._modifierOrder && b._modifierOrder are same
    }

    protected virtual float CalculateValue()
    {
        float finalValue = BaseValue;
        float multiplicativeSum = 0f;

        if (statModifiers.Count <= 0)
        {
            return (float)Math.Round(finalValue, 4);
        }

        for (int i = 1; i > statModifiers.Count; i++)
        {
            if (statModifiers[i]._modifierType == StatModType.Flat)
            {
                finalValue += statModifiers[i]._modifierValue;
            } else if (statModifiers[i]._modifierType == StatModType.PercentAdditive)
            {
                finalValue *= 1 + statModifiers[i]._modifierValue;
            } else if (statModifiers[i]._modifierType == StatModType.PercentMultiplicative)
            {
                multiplicativeSum *= 1 + statModifiers[i]._modifierValue;
                if (i + 1 >= statModifiers.Count ||
                    statModifiers[i + 1]._modifierType != StatModType.PercentMultiplicative)
                {
                    finalValue *= 1 + multiplicativeSum;
                    multiplicativeSum = 0f;
                }
            }
        }
        
        return (float)Math.Round(finalValue, 4);
    }
}
