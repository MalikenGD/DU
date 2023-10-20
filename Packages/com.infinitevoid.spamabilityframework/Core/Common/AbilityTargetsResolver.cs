using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Extensions;
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
        private const string LOG_MODULE = SpamLogModules.TARGET_RESOLVER;

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
        /// If the ability is an AOE-ability, it resolves targets on the hit-position (or hit area if directional) and performs an optional Line-of-sight check.
        /// If the ability is a single-target ability, sets the supplied target as a valid target.
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="hitPosition"></param>
        /// <param name="target"></param>
        public virtual void ResolveTargets(AbilityInvoker invoker, Vector3 hitPosition, IAbilityTarget target = null)
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
                SpamLogger.LogDebug(LOG_MODULE, "Not an AOE ability. Effect targets was set to target");
                ValidTargets[0] = target;
            }
        }
        
        private void GetValidTargetsInRadius(Vector3 hitPosition)
        {
            AreaOfEffectType areaType;
            float radius;
            Vector3 halfExtents;
            Vector3 physicsCenter;
            if (_ability is DirectionalAbilitySO directionalAbility)
            {
                radius = directionalAbility.Distance;
                areaType = directionalAbility.ConeAOE 
                    ? AreaOfEffectType.Sphere 
                    : AreaOfEffectType.Box;
                halfExtents = areaType == AreaOfEffectType.Sphere
                    ? new Vector3(radius, radius, radius)
                    : new Vector3(directionalAbility.Width, 1, directionalAbility.Distance);
                physicsCenter = areaType == AreaOfEffectType.Sphere
                    ? _invoker.transform.position
                    // When box casting directional abilities place a box at half distance
                    // from the caster since the box grows from the center.
                    : _invoker.transform.position.Add(z: directionalAbility.Distance / 2);
            }
            else
            {
                physicsCenter = hitPosition;
                areaType = _ability.AreaOfEffectType;
                radius = _ability.EffectRadius;
                halfExtents = new Vector3(radius, radius, radius);
            }

            var numColliders = areaType == AreaOfEffectType.Sphere
                ? Physics.OverlapSphereNonAlloc(physicsCenter, radius, _colliders, _ability.EffectLayers)
                : Physics.OverlapBoxNonAlloc(physicsCenter, halfExtents, _colliders, Quaternion.identity, _ability.EffectLayers);
            
            SpamLogger.LogDebug(LOG_MODULE, $"AOE effect. Cast found {numColliders.ToString()} colliders");
            var lookDirection = Vector3.zero;
            if(_invoker)
                lookDirection = hitPosition - _invoker.transform.position;
            int validTargetIdx = 0;
            for (int i = 0; i < numColliders; i++)
            {
                if(!_colliders[i].TryGetComponent<IAbilityTarget>(out var potentialTarget))
                    continue;

                if (_conditionConstraintsValidator.PreConditionsSatisfiedBy(potentialTarget) 
                    && _ability.TargetIsInSight(_invoker, potentialTarget, hitPosition, lookDirection))
                {
                    ValidTargets[validTargetIdx] = potentialTarget;
                    validTargetIdx++;
                }
            }
            SpamLogger.LogDebug(LOG_MODULE, $"After evaluation, {validTargetIdx.ToString()} targets where valid.");
        }
    }
}