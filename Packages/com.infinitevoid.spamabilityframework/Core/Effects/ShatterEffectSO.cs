using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Shatter", fileName = "shatterEffect.asset")]
    public class ShatterEffectSO : AbilityEffectSO
    {
        protected override string _metaHelpDescription => $@"Shatters the target into pieces. Requires that the target has an {nameof(IShatterable)}-component. You can create your own or use the supplied {nameof(ProcedualShatter)}-component."; 
        
        [Tooltip("Number of times each piece should be cut. A higher number equals more pieces but also more instantiations and destroys")]
        [SerializeField] private int _cutCascades = 3;
        [SerializeField] private int _explodeForce = 500;
        [SerializeField] private int _minPartDespawnSeconds = 3;
        [SerializeField] private int _maxPartDespawnSeconds = 5;

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
            if (!target.Transform) return;
            var shatter = target.Transform.GetComponent<IShatterable>();
            if (shatter == null)
            {
                SpamLogger.LogWarning($"{name}: {target.Transform.name} has no {nameof(IShatterable)}-component");
                return;
            }
            SpamLogger.LogDebug(_logModule, $"Shattering {target.Transform.name}");
            shatter.DestroyMesh(_cutCascades, _explodeForce, _minPartDespawnSeconds, _maxPartDespawnSeconds);
        }
    }
}