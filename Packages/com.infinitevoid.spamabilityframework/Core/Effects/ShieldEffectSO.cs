using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Shield", fileName = "shieldeffect.asset")]
    public class ShieldEffectSO : AbilityEffectSO
    {
        [SerializeField] private int _shieldValue;

        protected override string _metaHelpDescription =>
            $"Adds shield to the target. Requires the target to have an {nameof(IShieldable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var shieldable = target.Transform.GetComponent<IShieldable>();
            if (shieldable == null)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} does not have an {nameof(IShieldable)}-component");
                return;
            }

            SpamLogger.LogDebug(_logModule, $"Add shield to {target.Transform.name} with value {_shieldValue.ToString()}");
            shieldable.Shield(_shieldValue);
        }
    }
}