using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Damage", fileName = "damageeffect.asset")]
    public class DamageEffectSO : AbilityEffectSO
    {
        [SerializeField] private int _damageValue;
#if UNITY_EDITOR
        [SerializeField] private bool _logDamage = false;
#endif

        protected override string _metaHelpDescription =>
            $"Deals damage to the target. Requires the target to have an {nameof(IDamageable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var damageable = target.Transform.GetComponent<IDamageable>();

            if (damageable == null)
            {
                SpamLogger.LogWarning($"{target.Transform.name} does not have an {nameof(IDamageable)}-component");
                return;
            }
#if UNITY_EDITOR
            if (_logDamage)
                Debug.Log($"{invoker.transform.name} deals {_damageValue} damage to {target.Transform.name}");
#endif
            SpamLogger.LogDebug(_logModule, $"{invoker.transform.name} deals {_damageValue} damage to {target.Transform.name}");
            damageable.Damage(_damageValue);
        }
    }
}