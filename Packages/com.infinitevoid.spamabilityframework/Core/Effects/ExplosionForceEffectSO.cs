using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    /// <summary>
    /// Applies an explosion force when the ability hits. The ability has to be an AOE-ability for this to work
    /// </summary>
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Explosion force (AOE)",
        fileName = "explosionForceEffect.asset")]
    public class ExplosionForceEffectSO : AbilityEffectSO
    {
        [SerializeField] private float _force;
        [SerializeField] private float _upModifier;

        [Tooltip(
            "If this effect is used as a condition, this radius will be used since there's no ability AOE-radius to use")]
        [SerializeField]
        private float _radiusWhenCondition;

        protected override string _metaHelpDescription =>
            "(AOE-Effect) Adds explosion force to the target(s). Requires the ability to have an effect radius greater than 0 and the targets to have non-kinematic rigidbodies.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            if (!ability?.AOEAbility == false)
            {
                SpamLogger.LogWarning($"An Explosioneffect ({this.name}) is added to {ability.Name} but the ability is not an AOE ability.");
                return;
            }

            var rby = target.Transform.GetComponent<Rigidbody>();
            if (!rby)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} has no rigidbody attached.");
                return;
            }

            float radius = ability?.EffectRadius ?? _radiusWhenCondition;
            SpamLogger.LogDebug(LOG_MODULE, $"Adding {_force} force to {target.Transform.name} at {abilityPos.ToString()}, with radius {radius} and up {_upModifier}");
            rby.AddExplosionForce(_force, abilityPos, radius, _upModifier);
        }
    }
}