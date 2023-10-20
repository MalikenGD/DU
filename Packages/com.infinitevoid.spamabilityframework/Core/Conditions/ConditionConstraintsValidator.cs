using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    public class ConditionConstraintsValidator
    {
        private readonly IAbilityData _ability;

        public ConditionConstraintsValidator(IAbilityData ability)
        {
            this._ability = ability;
        }

        /// <summary>
        /// Returns if the given target satisfies all the preconditions on the given ability.
        /// Only checks for pre-conditions that shouldn't be checked on caster
        /// </summary>
        /// <param name="potentialTarget"></param>
        /// <returns></returns>
        public bool PreConditionsSatisfiedBy(IAbilityTarget potentialTarget)
        {
            if (!_ability.HasPreconditions) return true;

            if (!TryGetConditionTarget(potentialTarget, out var conditionTarget)) return false;

            for (int i = 0; i < _ability.PreConditions.Count; i++)
            {
                if(_ability.PreConditions[i].OnCaster) continue;
                if (!conditionTarget.Satisfies(_ability.PreConditions[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns if the given caster satisfies all the preconditions on the ability.
        /// Only checks for pre-conditions that should be checked on caster
        /// </summary>
        /// <param name="invoker">The caster the check preconditions on</param>
        /// <returns></returns>
        public bool CasterSatisfiesPreconditions(AbilityInvoker invoker)
        {
            if (!_ability.HasPreconditions) return true;

            if (!TryGetConditionTarget(invoker, out var conditionTarget)) return false;

            for (int i = 0; i < _ability.PreConditions.Count; i++)
            {
                if(!_ability.PreConditions[i].OnCaster) continue;
                if (!conditionTarget.Satisfies(_ability.PreConditions[i])) return false;
            }

            return true;
        }


        public bool ConstraintsSatisfiedBy(IReadOnlyList<ConditionConstraint> constraints,
            AbilityInvoker abilityInvoker, IAbilityTarget target)
        {
            if (!TryGetConditionTarget(target, out var conditionTarget)) return false;
            if (!TryGetConditionTarget(abilityInvoker, out var casterConditionTarget)) return false;

            for (int i = 0; i < constraints.Count; i++)
            {
                var actualTarget = (constraints[i].OnCaster ? casterConditionTarget : conditionTarget);
                if (!actualTarget.Satisfies(constraints[i])) return false;
            }

            return true;
 
        }

        private static bool TryGetConditionTarget(IAbilityTarget potentialTarget, out AbilityConditionsTarget targetConditions)
        {
            if (potentialTarget.Transform.TryGetComponent(out targetConditions)) return true;
            
            SpamLogger.LogDebug(SpamLogModules.CONDITION_CONSTRAINTS, $"Ability is trying to check conditions, but {potentialTarget.Transform.name} has no {nameof(AbilityConditionsTarget)}-component",
                potentialTarget.Transform.gameObject);

            return false;

        }
    }
}