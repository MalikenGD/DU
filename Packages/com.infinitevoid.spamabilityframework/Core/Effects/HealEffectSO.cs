using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Heal", fileName = "healeffect.asset")]
    public class HealEffectSO : AbilityEffectSO
    {
        [SerializeField] private int _healValue;

        protected override string _metaHelpDescription =>
            $"Heals the target. Requires the target to have an {nameof(IHealable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var healable = target.Transform.GetComponent<IHealable>();
            if (healable == null)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} does not have an {nameof(IHealable)}-component");
                return;
            }

            SpamLogger.LogDebug(LOG_MODULE, $"Healing {target.Transform.name} for {_healValue.ToString()}");
            healable.Heal(_healValue);
        }
    }
}