using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Add buff", fileName = "buffeffect.asset")]
    public class AddBuffEffectSO : AbilityEffectSO
    {
        [SerializeField] private ScriptableObject _buff;

        protected override string _metaHelpDescription =>
            $"Adds a buff to the target. Requires the target to have an {nameof(IBuffable)}-component and the buff to implement {nameof(IBuff)}.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var buffable = target.Transform.GetComponent<IBuffable>();
            if (buffable == null)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} has no {nameof(IBuffable)}-component");
                return;
            }

            if (!(_buff is IBuff buff))
            {
                SpamLogger.LogWarning($"{name}: the Buff supplies does not inherit from {nameof(IBuff)}");
                return;
            }

            SpamLogger.LogDebug(_logModule, $"Adding buff {_buff.name} to {target.Transform.name}");
            buffable.AddBuff(buff);
        }
    }
}