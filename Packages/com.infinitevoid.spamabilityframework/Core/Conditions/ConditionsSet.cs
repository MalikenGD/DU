using System;
using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    /// <summary>
    /// A set of conditions, handling satisfied-checks and lifetime of conditions that's added to it.
    /// </summary>
    public class ConditionsSet
    {
        public ActiveCondition this[int index] => _activeConditions[index];
        private ConditionsSet _conditionsSet;
        private readonly ActiveCondition[] _activeConditions;
        private readonly IAbilityTarget _abilityTarget;
        private AbilityConditionSO[] _immunities = null;
        private AbilityConditionSO[] _validConditions = null;
        private int _maxImunities;

        /// <summary>
        /// Raised when a condition was added. Float is lifetime. A lifetime of 0 means it's a permanent condition
        /// </summary>
        public event Action<AbilityConditionSO, float> ConditionAdded; 
        /// <summary>
        /// Raised when a condition was extended. Float is new lifetime
        /// </summary>
        public event Action<AbilityConditionSO, float> ConditionExtended; 
        public event Action<AbilityConditionSO> ConditionExpired;
        public event Action<AbilityConditionSO> ConditionRemoved;
        public event Action<AbilityConditionSO> ConditionTicked;

        public IReadOnlyList<ActiveCondition> ActiveConditions => _activeConditions;
        public AbilityConditionSO[] Immunities => _immunities;

        public ConditionsSet(int maxConditions, int maxImmunities = 1, IAbilityTarget abilityTarget = null)
        {
            _activeConditions = new ActiveCondition[maxConditions];
            _maxImunities = maxImmunities;
            _immunities = new AbilityConditionSO[maxImmunities];
            _abilityTarget = abilityTarget;
            for (int i = 0; i < maxConditions; i++)
            {
                _activeConditions[i] = ActiveCondition.None;
            }
        }

        public bool Satisfies(IReadOnlyList<ConditionConstraint> conditionConstraints)
        {
            for (var i = 0; i < conditionConstraints.Count; i++)
            {
                if (!Satisfies(conditionConstraints[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a condition to the set. If the set is at max capacity then the condition won't be added
        /// </summary>
        /// <param name="condition">The condition to add</param>
        /// <param name="appliedBy">The caster who applied the condition on the target</param>
        /// <param name="lifetime">The lifetime of the condition. If 0 it will stay in the set until overwritten och explicitly removed</param>
        public void Add(AbilityConditionSO condition, AbilityInvoker appliedBy, float lifetime)
        {
            var conditionAlreadyApplied = ConditionAlreadyApplied(condition, out var conditionAtIndex);
            if (condition.AddMultipleBehaviour == AddMultipleConditionBehaviour.DoNothing && conditionAlreadyApplied) return;

            if (!ConditionCanBeApplied(condition))
                return;
            
            
            if (condition.ExtendOnAddMultiple && conditionAlreadyApplied)
            {
                _activeConditions[conditionAtIndex].ExtendLifetime(lifetime);
                _activeConditions[conditionAtIndex].ApplySecondaryEffects(_abilityTarget, ConditionEffectEvent.Extended);
                ConditionExtended?.Invoke(condition, _activeConditions[conditionAtIndex].Lifetime);
                return;
            }

            if (condition.OverwriteOnAddMultiple && conditionAlreadyApplied)
            {
                _activeConditions[conditionAtIndex].SetLifetime(lifetime);
                _activeConditions[conditionAtIndex].ApplySecondaryEffects(_abilityTarget, ConditionEffectEvent.Added);
                ConditionExtended?.Invoke(condition, _activeConditions[conditionAtIndex].Lifetime);
                return;
            }

            for (int i = 0; i < _activeConditions.Length; i++)
            {
                if (_activeConditions[i] != ActiveCondition.None) continue;
                _activeConditions[i] = new ActiveCondition(condition, lifetime, appliedBy, ConditionTicked);
                _activeConditions[i].ApplySecondaryEffects(_abilityTarget, ConditionEffectEvent.Added);
                ConditionAdded?.Invoke(condition, lifetime);
                return;
            }
        }

        /// <summary>
        /// Returns if the given condition is in the list of valid conditions, or if there's no immunity to it.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool ConditionCanBeApplied(AbilityConditionSO condition)
        {
            if (_validConditions == null) return !ConditionIsInImmunities(condition);

            for (int i = 0; i < _validConditions.Length; i++)
            {
                if (_validConditions[i].IsSameAs(condition)) return true;
            }

            return false;
        }

        private bool ConditionIsInImmunities(AbilityConditionSO condition)
        {
            if (_immunities == null || _immunities.Length == 0) return false;

            for (int i = 0; i < _immunities.Length; i++)
            {
                if(!_immunities[i] || !_immunities[i].IsSameAs(condition)) continue;
                return true;
            }

            return false;
        }

        private bool ConditionAlreadyApplied(AbilityConditionSO abilityConditionSO, out int conditionAtIndex)
        {
            conditionAtIndex = -1;
            for (int i = 0; i < _activeConditions.Length; i++)
            {
                if (_activeConditions[i] == ActiveCondition.None) continue;
                if (!_activeConditions[i].IsSameConditionAs(abilityConditionSO)) continue;
                
                conditionAtIndex = i;
                return true;
            }

            return false;
        }

        public void TickLifetimes(float deltatime)
        {
            for (var i = 0; i < _activeConditions.Length; i++)
            {
                if (_activeConditions[i] == ActiveCondition.None) continue;
                var condition = _activeConditions[i];

                if (condition.IsPermanent) continue;

                condition.TickLifetime(deltatime);
                condition.ApplyTickedSecondaryEffects(_abilityTarget);
                if (!condition.IsExpired) continue;

                condition.ApplySecondaryEffects(_abilityTarget, ConditionEffectEvent.Expired);
                _activeConditions[i] = ActiveCondition.None;
                ConditionExpired?.Invoke(condition.Condition);
            }
        }

        public bool Satisfies(ConditionConstraint conditionConstraint)
        {
            var shouldHaveCondition = conditionConstraint.Constraint == ConditionConstraintType.Has;
            for (var index = 0; index < _activeConditions.Length; index++)
            {
                if (_activeConditions[index] == ActiveCondition.None) continue;
                if (_activeConditions[index].IsSameConditionAs(conditionConstraint.Condition))
                    return shouldHaveCondition;
            }

            return !shouldHaveCondition;
        }

        public bool HasActiveConditions()
        {
            for (int i = 0; i < _activeConditions.Length; i++)
            {
                if (_activeConditions[i] != ActiveCondition.None) return true;
            }

            return false;
        }

        public bool HasCondition(AbilityConditionSO condition)
        {
            for (int i = 0; i < _activeConditions.Length; i++)
            {
                if (_activeConditions[i] == ActiveCondition.None) continue;
                if (_activeConditions[i].IsSameConditionAs(condition)) return true;
            }

            return false;

        }

        /// <summary>
        /// Removes the given condition from the set.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>True if a conditions was removed, false if no condition of the given type was present</returns>
        public void RemoveCondition(AbilityConditionSO condition)
        {
            if (!HasCondition(condition)) return;

            for (int i = 0; i < _activeConditions.Length; i++)
            {
                if (_activeConditions[i] == ActiveCondition.None) continue;

                var activeCondition = _activeConditions[i];
                if (!activeCondition.IsSameConditionAs(condition)) continue;

                activeCondition.ApplySecondaryEffects(_abilityTarget, ConditionEffectEvent.Removed);
                _activeConditions[i] = ActiveCondition.None;
                ConditionRemoved?.Invoke(activeCondition.Condition);
            }
        }

        /// <summary>
        /// Sets the initial immunities to the conditions set.
        /// Will raise maximum number of immunities allowed if the given array is larger than initial maximum number of immunities allowed.
        /// </summary>
        /// <param name="initialImmunities"></param>
        public void SetInitialImmunities(AbilityConditionSO[] initialImmunities)
        {
            _maxImunities = _maxImunities < initialImmunities.Length ? initialImmunities.Length : _maxImunities;
            _immunities = new AbilityConditionSO[_maxImunities];
            for (int i = 0; i < initialImmunities.Length; i++)
                _immunities[i] = initialImmunities[i];
        }
        
        /// <summary>
        /// Permanently sets what conditions that will be allowed (for this instance)
        /// </summary>
        /// <param name="validConditions"></param>
        public void SetValidConditions(AbilityConditionSO[] validConditions)
        {
            _validConditions = validConditions;
        }

        /// <summary>
        /// Adds the given condition as an immunity if the given conditions is not already an immunity
        /// </summary>
        /// <param name="condition"></param>
        public void AddImmunity(AbilityConditionSO condition)
        {
            if (HasImmunity(condition)) return;
            AddImmunityUnchecked(condition);
        }

        /// <summary>
        /// Adds the given condition to immunities without checking if the set already contains an immunity for the condition.
        /// Use with care since this can add duplicates to immunities, and duplicate immunities won't be removed when removing immunities.
        /// </summary>
        /// <param name="condition"></param>
        public void AddImmunityUnchecked(AbilityConditionSO condition)
        {
            for (int i = 0; i < _maxImunities; i++)
            {
                if(_immunities[i]) continue;
                _immunities[i] = condition;
                return;
            }
            #if UNITY_EDITOR
            Debug.LogWarning("Trying to add immunity but immunities at max capacity");
            #endif
        }

        /// <summary>
        /// Returns if the set contains the given community
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool HasImmunity(AbilityConditionSO condition)
        {
            if (_immunities == null || _immunities.Length == 0) return false;
            for (int i = 0; i < _immunities.Length; i++)
            {
                if(_immunities[i] != null && _immunities[i].IsSameAs(condition)) return true;
            }

            return false;
        }

        public void RemoveImmunity(AbilityConditionSO condition)
        {
            for (int i = 0; i < _immunities.Length; i++)
            {
                if(_immunities[i] == null || !_immunities[i].IsSameAs(condition)) continue;
                _immunities[i] = null;
                return;
            }
        }
    }
}