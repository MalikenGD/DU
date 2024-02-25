using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

// Custom Indexing for Custom Ordering (ex: modifier custom ordering between flat and percentAdd)
public enum StatModType 
{
    Flat = 100,
    PercentAdditive = 200,
    PercentMultiplicative = 300
}

public class StatModifier
{
    public readonly float _modifierValue = 0f;
    public readonly StatModType _modifierType;
    public readonly int _modifierOrder;
    public readonly object _modifierSource;

    public StatModifier(StatModType type, float value, int order, object source)
    {
        _modifierType = type;
        _modifierValue = value;
        _modifierOrder = order;
        _modifierSource = source;
    }

    public StatModifier(StatModType type, float value) : this(type, value, (int) type, null){}
    
    public StatModifier(StatModType type, float value, int order) : this(type, value, order, null){}
    
    public StatModifier(StatModType type, float value, object source) : this(type, value, (int) type, source){}
}
