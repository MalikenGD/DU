using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// Handles getting valid <see cref="IAbilityTarget"/>s for a given ability.
    /// </summary>
    [Serializable]
    public class AbilityTargetsResolver
    {
        /// <summary>
        /// Contains the valid targets gotten after calling <see cref="ResolveTargets"/>.
        /// Targets will be added from the beggining up to the max number of targets for the ability, and if
        /// less targets than max are found then remaining elements will be null.
        /// I.e. if 2 targets were resolved then Targets[2] - Targets[max] will be null.
        /// </summary>
        public IAbilityTarget[] ValidTargets { get; private set; }
        private IAbilityData _ability;
        private AbilityInvoker _invoker;
        private Collider[] _colliders;
        private readonly ConditionConstraintsValidator _conditionConstraintsValidator;
        private const string _logModule = "[TargetResolver]";

        public AbilityTargetsResolver(IAbilityData ability, ConditionConstraintsValidator constraintsValidator)
        {
            this._ability = ability;
            if (ability.AOEAbility)
                _colliders = new Collider[ability.MaxEffectTargets];

            ValidTargets = new IAbilityTarget[_ability.MaxEffectTargets];
            _conditionConstraintsValidator = constraintsValidator;
        }

        /// <summary>
        /// Resolves valid targets, and saves them in <see cref="ValidTargets"/>.
        /// If the ability is an AOE-ability, it resolves targets on the hit-position and performs an optional Line-of-sight check.
        /// If the ability is a single-target ability, sets the supplied target as a valid target.
        /// I a target is supplied 
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="hitPosition"></param>
        /// <param name="target"></param>
        public void ResolveTargets(AbilityInvoker invoker, Vector3 hitPosition, IAbilityTarget target = null)
        {
            this._invoker = invoker;
#if UNITY_EDITOR
            // This is only done in editor as the number of targets should be changeable during runtime
            ValidTargets = new IAbilityTarget[_ability.MaxEffectTargets];
#else
            Array.Clear(ValidTargets,0,ValidTargets.Length);
#endif
            if (_ability.AOEAbility)
            {
                GetValidTargetsInRadius(hitPosition);
            }
            else if (target != null)
            {
                SpamLogger.LogDebug(_logModule, "Not an AOE ability. Effect targets was set to target");
                ValidTargets[0] = target;
            }
        }
        
        private void GetValidTargetsInRadius(Vector3 hitPosition)
        {
            float radius;
            if (_ability is DirectionalAbilitySO directionalAbility)
                radius = directionalAbility.Distance;
            else
                radius = _ability.EffectRadius;

            var numColliders = _ability.AreaOfEffectType == AreaOfEffectType.Sphere
                ? Physics.OverlapSphereNonAlloc(hitPosition, radius, _colliders, _ability.EffectLayers)
                : Physics.OverlapBoxNonAlloc(hitPosition, new Vector3(radius, radius, radius), _colliders, Quaternion.identity, _ability.EffectLayers);
            
            SpamLogger.LogDebug(_logModule, $"AOE effect. Cast found {numColliders.ToString()} colliders");
            var lookDirection = Vector3.zero;
            if(_invoker)
                lookDirection = hitPosition - _invoker.transform.position;
            int validTargetIdx = 0;
            for (int i = 0; i < numColliders; i++)
            {
                if(!_colliders[i].TryGetComponent<IAbilityTarget>(out var potentialTarget))
                    continue;

                if (_ability.TargetIsInSight(_invoker, potentialTarget, hitPosition, lookDirection)
                    && _conditionConstraintsValidator.PreConditionsSatisfiedBy(potentialTarget))
                {
                    ValidTargets[validTargetIdx] = potentialTarget;
                    validTargetIdx++;
                }
            }
            SpamLogger.LogDebug(_logModule, $"After evaluation, {validTargetIdx.ToString()} targets where valid.");
        }
    }
}