using System.Globalization;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Stun", fileName = "stuneffect.asset")]
    public class StunEffectSO : AbilityEffectSO
    {
        [SerializeField] private float _stunTime;

        protected override string _metaHelpDescription =>
            $"Stuns the target for the given time. Requires the target to have an {nameof(IStunable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var stunable = target.Transform.GetComponent<IStunable>();
            if (stunable == null)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} does not have an {nameof(IStunable)}-component");
                return;
            }

            SpamLogger.LogDebug(_logModule, $"Apply stun to {target.Transform.name} for time {_stunTime.ToString(CultureInfo.InvariantCulture)}");
            stunable.Stun(_stunTime);
        }
    }
}