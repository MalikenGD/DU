using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    public interface IAbilityData
    {
        bool TargetIsInSight(AbilityInvoker invoker, IAbilityTarget potentialTarget,
            Vector3 hitPosition, Vector3 lookDirection);

        string Name { get; }
        List<EffectAndTime> AbilityEffects { get; }
        float EffectRadius { get; }
        bool AOEAbility { get; }
        LayerMask EffectLayers { get; }
        int MaxEffectTargets { get; }
        float PerTargetEffectApplicationDelay { get; }
        bool ApplyPerTarget { get; }
        bool ApplyPerEffect { get; }
        List<ConditionConstraint> PreConditions { get; }
        bool HasPreconditions { get; }
        List<ConditionalEffectsSO> ConditionalEffects { get; }
        bool ConditionalEffectsBeforeMainEffects { get; }
        bool ConditionalEffectsAfterMainEffects { get; }
        AreaOfEffectType AreaOfEffectType { get; }
    }
}