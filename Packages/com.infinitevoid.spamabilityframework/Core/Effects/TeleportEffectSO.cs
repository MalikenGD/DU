using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability effects/Teleport", fileName = "teleporteffect.asset")]
    public class TeleportEffectSO : AbilityEffectSO
    {
        [SerializeField] private bool _targetToCaster;
        [SerializeField] private bool _casterToTarget;
        [HelpBox("Determines how far away from the target/caster the end position will be. If set to 0 the teleportee will be moved to the exact position of the other. A negative value will make the teleportee end up behind the target.")]
        [SerializeField] private float _offset;
        protected override string _metaHelpDescription => $"Teleports either the target or the caster to the other's location. Can be used as a swap-position effect if both caster and target are teleported. Requires the teleportee(s) to have an {nameof(ITeleportable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker)
        {
#if UNITY_EDITOR
            if (!_targetToCaster && !_casterToTarget)
            {
                SpamLogger.LogError($"{name}: Cannot teleport - neither target to caster or caster to target is set.");
                return;
            }
#endif
            var targetPos = target.Transform.position;
            var casterPos = invoker.Position;
            if (_casterToTarget && invoker.TryGetComponent<ITeleportable>(out var teleportable))
            {
                SpamLogger.LogDebug(LOG_MODULE, $"Teleport {invoker.Transform.name} to {target.Transform.name}");
                var offset = (casterPos - targetPos).normalized * _offset;
                teleportable.TeleportTo(targetPos + offset);
            }

            if (_targetToCaster && target.Transform.TryGetComponent<ITeleportable>(out var targetTeleportable))
            {
                SpamLogger.LogDebug(LOG_MODULE, $"Teleport {target.Transform.name} to {invoker.Transform.name}");
                var offset = (targetPos - casterPos).normalized * _offset;
                targetTeleportable.TeleportTo(casterPos + offset);
            }
        }
    }
}