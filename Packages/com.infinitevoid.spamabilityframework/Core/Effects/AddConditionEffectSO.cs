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
    public class AddConditionEffectSO : AbilityEffectSO
    {
        [SerializeField] private AbilityConditionSO _condition;
        [HelpBox("A lifetime of 0 means it will stay on the target until manually removed.")]
        [SerializeField] private float _lifetime;

        protected override string _metaHelpDescription =>
            $"Adds a condition to the target. Requires the target to have an {nameof(IAbilityConditionsTarget)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability, 
            AbilityInvoker invoker)
        {
            if(!target.Transform.TryGetComponent<IAbilityConditionsTarget>(out var abilityConditions))
            {
                SpamLogger.EditorOnlyLog($"{name}: {target.Transform.name} has no {nameof(IAbilityConditionsTarget)}-component");
                return;
            }   

            abilityConditions.AddCondition(_condition, _lifetime, invoker);
        }
    }
}