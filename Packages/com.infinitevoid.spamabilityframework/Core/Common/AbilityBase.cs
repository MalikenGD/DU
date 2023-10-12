﻿using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Audio;
using InfiniteVoid.SpamFramework.Core.Common.VFX;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// Base for all abilities. Contains basic and common functionality. This is used in favor of an interface
    /// to make it play nice with serialized fields in the inspector
    /// </summary>
    public abstract class AbilityBase : MonoBehaviour
    {
        [SerializeField] private AbilityInvoker _invoker;

        [Header("Event controls")]
        [Tooltip("Turn this off if the cooldown is short and you don't want event-spamming")]
        [SerializeField]
        private bool _raiseCooldownEvent = true;

        [Tooltip("Turn this off if the cooldown is short and you don't want event-spamming")] [SerializeField]
        private bool _raiseUsedEvent = true;

        [Header("VFX")] [SerializeField] private Transform _warmupVfxSpawnPosition;


        protected abstract AbilityBaseSO Ability { get; }
        protected AbilityInvoker Invoker => _invoker;

        protected virtual void OnCast()
        {
            PlayCastSFX();
            AbilityHit(new ImpactPoint(_castTargetPos), _target);
        }

        /// <summary>
        /// Should the ability cache on hit VFX?
        /// </summary>
        protected virtual bool CacheOnHitVfx => true;

        /// <summary>
        /// Called from <see cref="Start"/> to allow inheriting classes to implement start functionality.
        /// The base class sets up dependencies.
        /// </summary>
        protected virtual void OnStart()
        {
        }

        protected virtual void CleanupOnDestroy()
        {
        }

        public static implicit operator AbilityBaseSO(AbilityBase ab) => ab.Ability;

        public bool IsOnCooldown => _onCooldown;
        public float CooldownTimeLeft => _cooldownTimeLeft;
        public bool IsAoeAbility => Ability.AOEAbility;
        public float AreaOfEffectRadius => Ability.EffectRadius;
        public float CastTime => Ability.CastTime;
        public Sprite Icon => Ability.Icon;
        public string Name => Ability.Name;
        public string Description => Ability.Description;

        /// <summary>
        /// The effects an ability will apply. This is filtered for only public effects so it can be shown in the UI to the Player.
        /// </summary>
        public IReadOnlyList<AbilityEffectSO> Effects
        {
            get
            {
#if UNITY_EDITOR
                // To make it easier to debug we do this with LINQ in editor. This makes it possible to
                // turn effects publicity on and off to test.
                return Ability.AbilityEffects.Where(x => x.Effect.IncludedInAbilityEffects).Select(x => x.Effect)
                    .ToArray();
#endif
#if !UNITY_EDITOR
                return _abilityEffects;
#endif
            }
        }

        /// <summary>
        /// Returns if the ability needs to be manually stopped or not.
        /// This is true when the ability uses a bool as a cast animator parameter.
        /// </summary>
        public bool ContinuousCast => Ability.AnimationTimings != null && Ability.AnimationTimings.ContinuousCast;

        // Cached fields to avoid GC
        private Action _warmupAction;
        private Action _castAction;
        private Action _cooldownAction;
#if !UNITY_EDITOR
        private List<AbilityEffectSO> _abilityEffects = new List<AbilityEffectSO>();
#endif

        // Components
        private IAbilityVFXHandler _abilityVfxHandler;
        private AbilitySFXHandler _abilitySfxHandler;
        private AbilityTargetsResolver _targetResolver;
        private AbilityEffectsApplier _abilityEffectsApplier;

        private float _cooldownTimeLeft;
        private bool _onCooldown;
        private Vector3 _castTargetPos;
        private IAbilityTarget _target;
        private ConditionConstraintsValidator _conditionConstraintsValidator;

        private void Awake()
        {
            CacheActions();
        }

        private void CacheActions()
        {
            _warmupAction = WarmupAction;
            _castAction = OnCast;
            _cooldownAction = SetAbilityOnCooldown;
        }


        private void Start()
        {
            OnStart();
#if UNITY_EDITOR
            if (!_invoker)
            {
                Debug.LogError($"No invoker added to {this.name} on {this.gameObject.name}");
                this.enabled = false;
                return;
            }

            if (!Ability)
            {
                Debug.LogError($"Ability is null in {this.name}", this);
                this.enabled = false;
                return;
            }
#endif
#if !UNITY_EDITOR
                for(int i = 0; i < Ability.AbilityEffects.Count; i++)
                    if(Ability.AbilityEffects[i].Effect.IncludedInAbilityEffects)
                        _abilityEffects.Add(Ability.AbilityEffects[i].Effect);
#endif
            Ability.CacheAnimationTriggers();
            SetupDependencies();

            this.enabled = false;
        }

        /// <summary>
        /// Called from <see cref="Start"/> to setup all dependencies in the ability, effects, sfx, targetresolver and vfx.
        /// </summary>
        private void SetupDependencies()
        {
            _conditionConstraintsValidator = new ConditionConstraintsValidator(Ability);
            _abilityEffectsApplier = new AbilityEffectsApplier(Ability, _conditionConstraintsValidator);
            _abilitySfxHandler = new AbilitySFXHandler(_invoker.AudioSource, Ability.SFX);
            _targetResolver = new AbilityTargetsResolver(Ability, _conditionConstraintsValidator);

            if (Ability.VFX == null)
            {
                _abilityVfxHandler = NullAbilityVFXHandler.Instance;
            }
            else
            {
                _abilityVfxHandler = CacheOnHitVfx
                    ? new AbilityVFXHandler(Ability,
                        getWarmupVfxInstance: () => Instantiate(Ability.VFX.WarmupVfx, _warmupVfxSpawnPosition),
                        getOnHitVfxInstance: () => Instantiate(Ability.VFX.OnHitVfx, transform.position, Quaternion.identity).transform)
                    : new AbilityVFXHandler(Ability,
                        getWarmupVfxInstance: () => Instantiate(Ability.VFX.WarmupVfx, _warmupVfxSpawnPosition),
                        getOnHitVfxInstance: null);
            }
        }

        private void Update()
        {
            if (_onCooldown)
                HandleCooldown();
            if (_abilityEffectsApplier?.EffectsApplied == false)
                _abilityEffectsApplier.Update(Time.deltaTime);
            if (_abilityVfxHandler?.VfxApplied == false)
                _abilityVfxHandler.Update(Time.deltaTime);

            this.enabled = _onCooldown
                           || _abilityEffectsApplier is { EffectsApplied: false }
                           || _abilityVfxHandler is { VfxApplied: false }
                           || !_invoker.DoneInvoking;


            void HandleCooldown()
            {
                if (!Ability.AutomaticCooldown)
                    return;

                _cooldownTimeLeft -= Time.deltaTime;
                if (_cooldownTimeLeft <= 0)
                {
                    _onCooldown = false;
                }
            }
        }

        /// <summary>
        /// Stops casting the ability, which stops all timers and animations
        /// </summary>
        public void StopCasting()
        {
            _invoker.StopCasting();
            _abilityVfxHandler.StopWarmupVfx();
        }

        /// <summary>
        /// Ticks the abilities cooldown by the given amount. Does nothing if the ability has automatic cooldown handling.
        /// </summary>
        /// <param name="timeToTick"></param>
        public void TickCooldown(float timeToTick)
        {
            if (Ability.AutomaticCooldown) return;
            this._cooldownTimeLeft -= timeToTick;
            this._onCooldown = 0 < _cooldownTimeLeft;
            AbilitySystemEvents.RaiseAbilityCooldownTick(Ability, timeToTick);
        }

        /// <summary>
        /// Ticks all manually handled lifetimes of the ability, not including cooldown. To tick cooldown use <see cref="TickCooldown"/>
        /// </summary>
        /// <param name="timeToTick"></param>
        public void TickLifetime(float timeToTick)
        {
            _abilityVfxHandler?.TickOnHitVFX(timeToTick);
        }

        public void SetWarmupVFXSpawn(Transform spawnPoint) => this._warmupVfxSpawnPosition = spawnPoint;

        /// <summary>
        /// Casts the ability in the given direction or at the given position depending on the type of ability 
        /// </summary>
        /// <param name="pos"></param>
        public virtual void Cast(Vector3 pos)
        {
            CastAbility(pos);
        }

        /// <summary>
        /// Casts the ability if all checks (range, cooldown...) are successful.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="target"></param>
        protected void CastAbility(Vector3 targetPosition, IAbilityTarget target = null)
        {
            if (!CanCastAbility(targetPosition)) return;

            #if UNITY_EDITOR
            if(_warmupAction == null)
                CacheActions(); // When recompiling during play the actions might be set to null.
            #endif
            _castTargetPos = targetPosition;
            _target = target;
            _invoker.CastAbility(Ability, _warmupAction, _castAction, _cooldownAction);
            if (_raiseUsedEvent)
                AbilitySystemEvents.RaiseAbilityUsed(Ability, _castTargetPos);
            this.enabled = true;
        }

        /// <summary>
        /// Plays SFX and VFX and applies all effects in an AOE (if the ability is an AOE-ability) at the given position and/or to the given target
        /// </summary>
        /// <param name="impactPoint"></param>
        /// <param name="target"></param>
        protected void AbilityHit(ImpactPoint impactPoint, IAbilityTarget target = null)
        {
            _targetResolver.ResolveTargets(_invoker, impactPoint.Position, target);
            _abilityVfxHandler.ExecuteOnHitVFX(impactPoint, _targetResolver.ValidTargets);
            _abilityEffectsApplier.ApplyEffects(impactPoint.Position, _targetResolver.ValidTargets, _invoker);
            _abilitySfxHandler.PlayOnHitSfx(target);
        }

        protected void PlayCastSFX() => _abilitySfxHandler?.PlayCastSfx();

        /// <summary>
        /// Executes the warmup action, i.e. the VFX and SFX
        /// </summary>
        private void WarmupAction()
        {
            _abilityVfxHandler?.ExecuteWarmupVFX();
            _abilitySfxHandler?.PlayWarmupSfx();
        }

        /// <summary>
        /// Returns if the ability can be cast
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool CanCastAbility(Vector3 target)
        {
            if (!_invoker.CanCastAbility(Ability, target))
                return false;
            if (_onCooldown)
            {
                if (!_raiseCooldownEvent)
                    return false;

                SpamLogger.EditorOnlyLog($"{this.Ability.name} is on cooldown");
                AbilitySystemEvents.RaiseAbilityIsOnCooldown(Ability);
                return false;
            }

            if (!_conditionConstraintsValidator.CasterSatisfiesPreconditions(Invoker)) return false;

            return true;
        }

        /// <summary>
        /// Sets the ability on cooldown.
        /// </summary>
        private void SetAbilityOnCooldown()
        {
            if (Ability.Cooldown <= 0) return;
            if (Ability.AutomaticCooldown)
            {
                this.enabled = true;
            }

            _onCooldown = true;
            _cooldownTimeLeft = Ability.Cooldown;
            AbilitySystemEvents.RaiseAbilityCooldownStart(Ability);
        }

        private void OnDestroy()
        {
            _abilityVfxHandler?.Destroy(Destroy);
            CleanupOnDestroy();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Tries to get an <see cref="AbilityInvoker"/> from this gameobject. In no <see cref="AbilityInvoker"/> can be found it searches for an in parents and children of this gameobject
        /// </summary>
        [ContextMenu("Get from hierarchy")]
        public void GetInvokerFromHierarchy()
        {
            if(!TryGetComponent(out _invoker))
                _invoker = transform.GetComponentInHierarchy<AbilityInvoker>();
        }
#endif
    }
}