using System;
using System.Collections;
using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using Unity.VisualScripting;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    [SerializeField] private TargetedAbility _ability;

    public void CastAbility(IAbilityTarget target)
    {
        if (_ability.RequiresTarget)
        {
            if (target != null)
            {
                _ability.Cast(target);
            }
        }
    }
}
