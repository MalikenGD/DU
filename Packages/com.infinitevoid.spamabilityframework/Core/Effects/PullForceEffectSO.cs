using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    /// <summary>
    /// Applies an pull force when the ability hits.
    /// </summary>
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Pull force", fileName = "explosionForceEffect.asset")]
    public class PullForceEffectSO : AbilityEffectSO
    {
        [SerializeField] private float _force;
        [SerializeField] private bool _pullTowardsCaster;

        protected override string _metaHelpDescription =>
            "Pulls the target towards the ability's hitpoint (or the caster). Requires the target to have a non-kinematic rigidbody.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            var rby = target.Transform.GetComponent<Rigidbody>();
            if (!rby)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} has no rigidbody attached.");
                return;
            }

            var direction = ((_pullTowardsCaster ? invoker.Position : abilityPos) - rby.position).normalized;
            SpamLogger.LogDebug(LOG_MODULE, $"Adding {(direction * _force).ToString()} force to {target.Transform.name} (direction {direction} force {_force}) in pulleffect");
            rby.AddForce(direction * _force, ForceMode.Impulse);
        }
    }
}