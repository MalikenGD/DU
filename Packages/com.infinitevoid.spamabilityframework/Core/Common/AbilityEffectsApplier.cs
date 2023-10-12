using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// Handles applying an ability's effects to a given list of targets.
    /// </summary>
    [Serializable]
    public class AbilityEffectsApplier
    {
        public bool EffectsApplied => _curState == ApplyEffectsState.Done;
        public float MaxEffectTime { get; private set; }

        private IAbilityTarget[] _validTargets;
        private IAbilityData _ability;
        private AbilityInvoker _invoker;
        private Vector3 _hitpos;
        private float _timeElapsed = 0;
        private ApplyEffectsState _curState = ApplyEffectsState.Done;
        private int _curTargetIndex = 0;
        private IAbilityTarget _curTarget;
        private EffectAndTime _curEffect;
        private int _curEffectIndex = 0;
        private readonly ConditionConstraintsValidator _conditionConstraintsValidator;
        
        private const string _logModule = "[EffectsApplier]";

        public AbilityEffectsApplier(AbilityBaseSO ability, ConditionConstraintsValidator constraintsValidator)
        {
            this._ability = ability;
            float timePerEffect = 0f;
            for (int i = 0; i < ability.AbilityEffects.Count; i++)
            {
                timePerEffect += ability.AbilityEffects[i].EffectTime;
            }

            MaxEffectTime = ability.ApplyPerTarget
                ? (ability.PerTargetEffectApplicationDelay * ability.MaxEffectTargets) +
                  (timePerEffect * ability.MaxEffectTargets)
                : timePerEffect * ability.MaxEffectTargets;
            _conditionConstraintsValidator = constraintsValidator;
        }


        /// <summary>
        /// Applies all ability-effects to the given targets.
        /// </summary>
        /// <param name="hitPosition"></param>
        /// <param name="targets"></param>
        /// <param name="invoker"></param>
        public void ApplyEffects(Vector3 hitPosition, IAbilityTarget[] targets, AbilityInvoker invoker)
        {
            SpamLogger.LogDebug(_logModule, "Start applying effects");
            _invoker = invoker;
            _hitpos = hitPosition;
            _validTargets = targets;
            _curState = ApplyEffectsState.Initialize;
        }

        public void Update(float deltaTime)
        {
            // This was first implemented as a coroutine but that generated ~80b of garbage every cast even with MEC.
            // To keep lib garbage free this is now a state machine, which makes it less readable but has other upsides
            // as operations are spread over frames for less calculations each frame.
            switch (_curState)
            {
                case ApplyEffectsState.Done:
                    _curEffectIndex = 0;
                    _curTargetIndex = 0;
                    return;
                case ApplyEffectsState.Initialize:
                    _curState = Initialize();
                    return;
                case ApplyEffectsState.GetNextTarget:
                    _curState = GetNextTarget();
                    return;
                case ApplyEffectsState.NoMoreTargets:
                    _curState = NoMoreTargets();
                    return;
                case ApplyEffectsState.GetNextEffect:
                    _curState = GetNextEffect();
                    return;
                case ApplyEffectsState.NoMoreEffects:
                    _curState = NoMoreEffects();
                    return;
                case ApplyEffectsState.ApplyEffectToTarget:
                    _curState = ApplyEffectToTarget();
                    return;
                case ApplyEffectsState.WaitForNextEffect:
                    _curState = WaitForNextEffect(deltaTime);
                    return;
                case ApplyEffectsState.WaitForNextTarget:
                    _curState = WaitForNextTarget(deltaTime);
                    return;
                default: return;
            }
        }

        private ApplyEffectsState WaitForNextTarget(float deltaTime)
        {
            _timeElapsed += deltaTime;
            if (_timeElapsed < _ability.PerTargetEffectApplicationDelay)
                return ApplyEffectsState.WaitForNextTarget;
            _timeElapsed = 0;
            return GetNextTarget();
        }

        private ApplyEffectsState WaitForNextEffect(float deltaTime)
        {
            _timeElapsed += deltaTime;
            if (_timeElapsed < _curEffect.EffectTime)
                return ApplyEffectsState.WaitForNextEffect;
            _timeElapsed = 0;
            return GetNextEffect();
        }

        private ApplyEffectsState ApplyEffectToTarget()
        {
            if (_curEffect.ApplyOnCaster)
            {
                SpamLogger.LogDebug(_logModule, $"Apply {_curEffect.Effect.Name} to {_invoker.Transform.name}");
                _curEffect.Effect.ApplyTo(_invoker, _hitpos, _ability, _invoker);
            }
            else
            {
                SpamLogger.LogDebug(_logModule, $"Apply {_curEffect.Effect.Name} to {_curTarget.Transform.name}");
                _curEffect.Effect.ApplyTo(_curTarget, _hitpos, _ability, _invoker);
            }

            if (_ability.ApplyPerTarget)
            {
                SpamLogger.LogDebug(_logModule, $"Apply per target. Set state to {ApplyEffectsState.WaitForNextEffect}");
                return ApplyEffectsState.WaitForNextEffect;
            }
            else
            {
                SpamLogger.LogDebug(_logModule, $"Apply per effect. Set state to {ApplyEffectsState.WaitForNextEffect}");
                return ApplyEffectsState.WaitForNextTarget;
            }
        }

        private ApplyEffectsState NoMoreEffects()
        {
            // If there's no more effects to apply and we don't
            // apply effects to each target in order we're done
            // since all effects have been applied
            if (!_ability.ApplyPerTarget)
            {
                SpamLogger.LogDebug(_logModule, "Apply effects done. No more effects and not applying per target.");
                return ApplyEffectsState.Done;
            }

            // There's no more effects to apply to the current target which means we're done
            // with the current target. Get first effect and wait for next target
            _curEffectIndex = 0;
            _curEffect = _ability.AbilityEffects[_curEffectIndex];
            SpamLogger.LogDebug(_logModule, $"Set state to {ApplyEffectsState.WaitForNextTarget}");
            return ApplyEffectsState.WaitForNextTarget;
        }

        private ApplyEffectsState NoMoreTargets()
        {
            // If we apply effects per target and there's no more targets we're done
            if (_ability.ApplyPerTarget)
            {
                SpamLogger.LogDebug(_logModule, "Apply effects done. No more targets and effects applied by target.");
                return ApplyEffectsState.Done;
            }

            // Effects are applied per effect and there's no more targets. Current effect is done
            // as there's no more targets. Get the next effect.
            _curTargetIndex = 0;
            _curTarget = _validTargets[_curTargetIndex];
            return GetNextEffect();
        }

        private ApplyEffectsState GetNextEffect()
        {
            _curEffectIndex++;
            if (_ability.AbilityEffects.Count <= _curEffectIndex)
            {
                SpamLogger.LogDebug(_logModule, $"Set state to {ApplyEffectsState.NoMoreEffects}");
                return ApplyEffectsState.NoMoreEffects;
            }

            _curEffect = _ability.AbilityEffects[_curEffectIndex];
            if (!_curEffect.Effect)
            {
                SpamLogger.LogDebug(_logModule, $"Set state to {ApplyEffectsState.NoMoreEffects}");
                return ApplyEffectsState.NoMoreEffects;
            }
            else
            {
                SpamLogger.LogDebug(_logModule, $"Set state to {ApplyEffectsState.ApplyEffectToTarget}");
                return ApplyEffectsState.ApplyEffectToTarget;
            }
        }

        private ApplyEffectsState GetNextTarget()
        {
            if (ShouldApplyConditionalEffectsBeforeSwitchingTarget())
                ApplyConditionalEffects();

            _curTargetIndex++;
            if (_validTargets.Length <= _curTargetIndex)
            {
                SpamLogger.LogDebug(_logModule, $"Target was the last. Set state to {ApplyEffectsState.NoMoreTargets}");
                return ApplyEffectsState.NoMoreTargets;
            }

            _curTarget = _validTargets[_curTargetIndex];
            if (_curTarget == null)
            {
                SpamLogger.LogDebug(_logModule, $"Target was last or null. Set state to {ApplyEffectsState.NoMoreTargets}");
                return ApplyEffectsState.NoMoreTargets;
            }

            if (ShouldApplyConditionalEffectsAfterSwitchingTarget())
                ApplyConditionalEffects();
            return ApplyEffectsState.ApplyEffectToTarget;

            bool AtLastEffect() => _curEffectIndex == _ability.AbilityEffects.Count - 1;
            bool AtFirstEffect() => _curEffectIndex == 0;

            bool ShouldApplyConditionalEffectsBeforeSwitchingTarget()
            {
                if (_ability.ConditionalEffectsAfterMainEffects)
                    return _ability.ApplyPerTarget || AtLastEffect();
                return false;
            }

            bool ShouldApplyConditionalEffectsAfterSwitchingTarget()
            {
                if (_ability.ConditionalEffectsBeforeMainEffects)
                    return _ability.ApplyPerTarget || AtLastEffect();
                if (_ability.ConditionalEffectsBeforeMainEffects)
                    return _ability.ApplyPerEffect && AtFirstEffect();
                return false;
            }
        }

        private ApplyEffectsState Initialize()
        {
#if UNITY_EDITOR
            if (_ability == null)
            {
                SpamLogger.EditorOnlyLog("SPAM Framework: Recompilation of scripts during playmode is not supported. Restart playmode to fix this.");
                return ApplyEffectsState.Done;
            }
#endif
            _curTargetIndex = 0;
            _curEffectIndex = 0;
            if (_curTargetIndex < _validTargets.Length)
                _curTarget = _validTargets[_curTargetIndex];
            if (_curEffectIndex < _ability.AbilityEffects.Count)
                _curEffect = _ability.AbilityEffects[_curEffectIndex];
            if (_curTarget == null )
            {
                SpamLogger.LogWarning("Couldn't initialize ability effects applier: No targets.");
                return ApplyEffectsState.Done;
            }

            if (_curEffect == null || !_curEffect.Effect)
            {
                SpamLogger.LogWarning("Couldn't initialize ability effects applier: No effects to apply.");
                return ApplyEffectsState.Done;
            }
                

            _timeElapsed = 0;

            if (_ability.ConditionalEffectsBeforeMainEffects)
                ApplyConditionalEffects();
            SpamLogger.LogDebug(_logModule, $"Set state to {ApplyEffectsState.ApplyEffectToTarget}");
            return ApplyEffectsState.ApplyEffectToTarget;
        }

        private void ApplyConditionalEffects()
        {
            #if SPAM_DEBUG
            if(_curTarget.Transform)
                SpamLogger.LogDebug(_logModule,$"Apply conditional effects to {_curTarget.Transform.name}");
            #endif
            for (int i = 0; i < _ability.ConditionalEffects.Count; i++)
            {
                var conditionalEffect = _ability.ConditionalEffects[i];
                if (_conditionConstraintsValidator.ConstraintsSatisfiedBy(conditionalEffect.Constraints, _invoker, _curTarget))
                    conditionalEffect.ApplyEffects(_curTarget, _hitpos, _ability, _invoker);
            }
        }

        private enum ApplyEffectsState
        {
            Done,
            Initialize,
            ApplyEffectToTarget,
            GetNextTarget,
            GetNextEffect,
            WaitForNextEffect,
            WaitForNextTarget,
            NoMoreTargets,
            NoMoreEffects
        }
    }
}