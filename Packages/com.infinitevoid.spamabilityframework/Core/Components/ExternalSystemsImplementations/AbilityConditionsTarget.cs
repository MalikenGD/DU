using System;
using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations
{
    /// <summary>
    /// Component that allows the target to be affected by conditions.
    /// This is the main entry point to conditions on a given target.
    /// </summary>
    [RequireComponent(typeof(IAbilityTarget))]
    public class AbilityConditionsTarget : MonoBehaviour, IAbilityConditionsTarget
    {
        [Tooltip("The maximum number of conditions this target can have simultaneously. This is needed to avoid Garbage Collection at runtime.")]
        [SerializeField] private int _maxNumConditions = 5;

        [SerializeField] private TimeHandlingEnum _timeHandling = TimeHandlingEnum.Automatic;

        [Header("Immunities")] 
        [HelpBox("Immunities are conditions that won't affect the target. Can be added and remove programatically at runtime; Changing initial immunities at runtime has no effect.")]
        [Tooltip("The maximum number of immunities this target can have simultaneously. This is needed to avoid Garbage Collection at runtime.")]
        [SerializeField] private int _maxNumImmunities;
        [SerializeField] private AbilityConditionSO[] _initialImmunities = Array.Empty<AbilityConditionSO>();
        
        [Space(25)]
        [HelpBox("If set, then only these conditions will affect the target. Can't be changed at runtime. Note that immunities are only applicable when the target has no valid conditions.")]
        [SerializeField] private AbilityConditionSO[] _validConditions = Array.Empty<AbilityConditionSO>();
        
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _debugLogging;
#endif
        private ConditionsSet _conditions;
        private IAbilityTarget _abilityTarget;

        /// <summary>
        /// All active conditions along with their lifetime
        /// </summary>
        public IReadOnlyList<ActiveCondition> ActiveConditions => _conditions?.ActiveConditions;
        public IReadOnlyList<AbilityConditionSO> ValidConditions => _validConditions;
        public IReadOnlyList<AbilityConditionSO> InitialImmunities => _initialImmunities;
        public IReadOnlyList<AbilityConditionSO> ActiveImmunities => _conditions?.Immunities;

        /// <summary>
        /// Raised when a condition is added. Float is lifetime. A lifetime of 0 means the condition is permanent
        /// </summary>
        public event Action<AbilityConditionSO, float> ConditionAdded;

        /// <summary>
        /// Raised when a condition was extended, i.e. it's time was updated.
        /// Float is new lifetime
        /// </summary>
        public event Action<AbilityConditionSO, float> ConditionExtended;

        /// <summary>
        /// Raised when a condition is expired
        /// </summary>
        public event Action<AbilityConditionSO> ConditionExpired;

        /// <summary>
        /// Raised when a condition is removed 
        /// </summary>
        public event Action<AbilityConditionSO> ConditionRemoved;

        /// <summary>
        /// Raised when a condition "ticks", i.e. applies its effect with trigger set to "tick"
        /// </summary>
        public event Action<AbilityConditionSO> ConditionTicked;

        private void Awake()
        {
            TryGetComponent(out _abilityTarget);
            _conditions = new ConditionsSet(_maxNumConditions,_maxNumImmunities, _abilityTarget);
            if (_validConditions.Length <= 0)
                _conditions.SetInitialImmunities(_initialImmunities);
            else _conditions.SetValidConditions(_validConditions);
            _conditions.ConditionExpired += OnConditionExpired;
            _conditions.ConditionRemoved += OnConditionRemoved;
            _conditions.ConditionExtended += OnConditionExtended;
            _conditions.ConditionAdded += OnConditionAdded;
            _conditions.ConditionTicked += OnConditionTicked;
        }

        private void OnDestroy()
        {
            _conditions.ConditionExpired -= OnConditionExpired;
            _conditions.ConditionRemoved -= OnConditionRemoved;
            _conditions.ConditionExtended -= OnConditionExtended;
            _conditions.ConditionAdded -= OnConditionAdded;
            _conditions.ConditionTicked -= OnConditionTicked;
        }

        private void Update()
        {
            if (!_conditions.HasActiveConditions())
            {
                this.enabled = false;
                return;
            }

            _conditions.TickLifetimes(Time.deltaTime);
        }

        /// <summary>
        /// Ticks all conditions lifetime by the given deltaTime
        /// </summary>
        /// <param name="deltaTime"></param>
        public void TickLifetimes(float deltaTime) => _conditions.TickLifetimes(deltaTime);

        public bool HasCondition(AbilityConditionSO condition) => _conditions.HasCondition(condition);

        /// <summary>
        /// Returns if this target satisfies the given <see cref="ConditionConstraint"/>s.
        /// </summary>
        /// <param name="conditionConstraints">The list of constraints to evaluate</param>
        /// <returns></returns>
        public bool Satisfies(IReadOnlyList<ConditionConstraint> conditionConstraints)
        {
            if (_conditions.Satisfies(conditionConstraints)) return true;

#if UNITY_EDITOR
            Log("Target doesn't satisfy pre-conditions", this.gameObject);
#endif
            return false;
        }
        
        /// <summary>
        /// Returns if this target satisfies the given <see cref="ConditionConstraint"/>.
        /// </summary>
        /// <param name="conditionConstraints">The list of constraints to evaluate</param>
        /// <returns></returns>
        public bool Satisfies(ConditionConstraint conditionConstraints)
        {
            if (_conditions.Satisfies(conditionConstraints)) return true;

#if UNITY_EDITOR
            Log("Target doesn't satisfy pre-conditions", this.gameObject);
#endif
            return false;
        }

        /// <summary>
        /// Adds the given condition to the target with the given lifetime.
        /// Raises <see cref="ConditionAdded"/> if the condition was added as a new condition,
        /// and <see cref="ConditionExtended"/> if the conditions lifetime was updated.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="lifetime"></param>
        /// <param name="caster"></param>
        public void AddCondition(AbilityConditionSO condition, float lifetime, AbilityInvoker caster)
        {
            _conditions.Add(condition, caster, lifetime);
            this.enabled = _timeHandling == TimeHandlingEnum.Automatic;
        }

        /// <summary>
        /// Removes the given condition, if present, from the target.
        /// Raises <see cref="ConditionRemoved"/> if a condition was removed
        /// </summary>
        /// <param name="condition"></param>
        public void RemoveCondition(AbilityConditionSO condition) => _conditions.RemoveCondition(condition);

        /// <summary>
        /// Adds the given condition to the target's immunities.
        /// </summary>
        /// <param name="condition"></param>
        public void AddImmunity(AbilityConditionSO condition) => _conditions.AddImmunity(condition);
        
        /// <summary>
        /// Removes the given conditions from the target's immunities.
        /// </summary>
        /// <param name="condition"></param>
        public void RemoveImmunity(AbilityConditionSO condition) => _conditions.RemoveImmunity(condition);
        
        private void OnConditionAdded(AbilityConditionSO condition, float lifetime)
        {
            ConditionAdded?.Invoke(condition, lifetime);
        }

        private void OnConditionExtended(AbilityConditionSO condition, float newLifetime)
        {
            ConditionExtended?.Invoke(condition,newLifetime);
        }

        private void OnConditionExpired(AbilityConditionSO condition)
        {
            ConditionExpired?.Invoke(condition);
            if (!_conditions.HasActiveConditions())
                this.enabled = false;
        }

        private void OnConditionRemoved(AbilityConditionSO condition)
        {
            ConditionRemoved?.Invoke(condition);
            if (!_conditions.HasActiveConditions())
                this.enabled = false;
        }
        
        private void OnConditionTicked(AbilityConditionSO condition)
        {
            ConditionTicked?.Invoke(condition);
        }

        

#if UNITY_EDITOR
        private void Log(string message, GameObject go)
        {
            if (_debugLogging)
                Debug.Log(message, go);
        }
#endif
    }
}