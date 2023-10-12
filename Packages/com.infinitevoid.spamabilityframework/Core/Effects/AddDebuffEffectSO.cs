using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Add Debuff", fileName = "debuffeffect.asset")]
    public class AddDebuffEffectSO : AbilityEffectSO
    {
        [SerializeField] private ScriptableObject _debuff;

        protected override string _metaHelpDescription =>
            $"Adds a debuff to the target. Requires the target to have an {nameof(IDebuffable)}-component and the debuff to implement {nameof(IDebuff)}.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var debuffable = target.Transform.GetComponent<IDebuffable>();
            if (debuffable == null)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} has no {nameof(IDebuffable)}-component");
                return;
            }

            if (!(_debuff is IDebuff debuff))
            {
                SpamLogger.LogWarning($"{name}: the Buff supplies does not inherit from {nameof(IDebuff)}");
                return;
            }

            SpamLogger.LogDebug(_logModule, $"Adding debuff {_debuff.name} to {target.Transform.name}");
            debuffable.AddDebuff(debuff);
        }
    }
}