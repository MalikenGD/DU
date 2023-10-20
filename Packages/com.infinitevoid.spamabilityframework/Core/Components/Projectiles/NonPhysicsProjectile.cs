using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Projectiles
{
    [RequireComponent(typeof(Collider))]
    public class NonPhysicsProjectile : Projectile
    {
        [SerializeField] private CircleTelegraph _telegraph;
        private bool _telegraphed;
        private Collider _collider;

        protected override bool Is3dProjectile => true;
        protected override void OnAwake()
        {
            if (_telegraph)
                _telegraph.transform.SetParent(null);
            TryGetComponent(out _collider);
            if (!_collider.isTrigger)
            {
                SpamLogger.LogWarning($"Projectile {Transform.name} is not set to trigger, which will prevent it from being cast at a target. Setting collider to trigger for this session. Consider marking this projectile prefab's collider as a trigger to not have unexpected behaviour and get rid of this message.");
                _collider.isTrigger = true;
            }
        }

        protected override void OnInit(ProjectileAbilitySO ability) { }

        protected override void OnResetProjectile()
        {
            this._collider.enabled = true;
        }

        protected override void OnDeactivated()
        {
            if (_telegraph)
                _telegraph.Hide();
            _collider.enabled = false;
        }

        protected override void OnStopped()
        {
            if (_telegraph)
                _telegraph.Hide();
            _collider.enabled = false;
        }

        protected override void OnDestroyed()
        {
            if (_telegraph)
                Destroy(_telegraph.gameObject);
        }

        protected override void OnSetupForMove(AbilityInvoker projectileInvoker,
            ProjectileAbilitySO projectileSo,
            Vector3 target,
            ProjectileSettings settings,
            bool? telegraphedOverride)
        {
            this._telegraphed = projectileSo.TelegraphOnCast && _telegraph;
            if (telegraphedOverride.HasValue)
                _telegraphed = telegraphedOverride.Value;
            if (_telegraphed && _telegraph)
                _telegraph.Show(projectileSo.EffectRadius, target);
        }

        protected override void MoveProjectile(Vector3 velocity, Vector3 targetPosition, Quaternion desiredRotation,
            MovementTiming movementTiming)
        {
            var deltaTime = movementTiming == MovementTiming.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
            var newPos = Transform.position + (velocity * deltaTime);
            Transform.SetPositionAndRotation(newPos, desiredRotation);

            if (_telegraph)
                _telegraph.SetPosition(targetPosition);
        }

        private void OnTriggerEnter(Collider other)
        {
            LogIfProjectileHitCaster(other.transform.GetComponentInHierarchy<AbilityInvoker>());
            var target = base.GetTargetFromHierarchy(other.transform);
            base.HitTarget(target, -transform.forward);
        }
    }
}