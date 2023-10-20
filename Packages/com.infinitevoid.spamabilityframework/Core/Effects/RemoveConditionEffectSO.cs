using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Conditions;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Add condition", fileName = "addCondition.asset")]
    public class RemoveConditionEffectSO : AbilityEffectSO
    {
        [SerializeField] private AbilityConditionSO _condition;

        protected override string _metaHelpDescription =>
            $"Removes a condition from the target. Requires the target to have an {nameof(IAbilityConditionsTarget)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            if(!target.Transform.TryGetComponent<IAbilityConditionsTarget>(out var abilityConditions))
            {
                SpamLogger.EditorOnlyLog($"{name}: {target.Transform.name} has no {nameof(IAbilityConditionsTarget)}-component");
                return;
            }   

            abilityConditions.RemoveCondition(_condition);
        }
    }
}