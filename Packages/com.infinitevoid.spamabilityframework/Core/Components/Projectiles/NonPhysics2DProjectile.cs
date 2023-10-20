using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Projectiles
{
    [RequireComponent(typeof(Collider2D))]
    public class NonPhysics2DProjectile : Projectile
    {
        [SerializeField] private CircleTelegraph _telegraph;

        private bool _usePhysics;
        private bool _telegraphed;
        private Collider2D _collider;

        protected override bool Is3dProjectile => false;
        
        protected override void OnAwake()
        {
            TryGetComponent(out _collider);
            if (_telegraph)
                _telegraph.transform.SetParent(null);
            if (!_collider.isTrigger)
            {
                SpamLogger.LogWarning(
                    $"Projectile {Transform.name} is not set to trigger, which will prevent it from being cast at a target. Setting collider to trigger for this session. Consider marking this projectile prefab's collider as a trigger to not have unexpected behaviour and get rid of this message.");
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
            var position = Transform.position += velocity * deltaTime;
            if (0f < desiredRotation.w) desiredRotation = new Quaternion(desiredRotation.x, desiredRotation.y, desiredRotation.z, desiredRotation.w);
            var rotation = Quaternion.Euler(0, 0, Mathf.Atan2(desiredRotation.z, desiredRotation.w) * Mathf.Rad2Deg);
            Transform.SetPositionAndRotation(position, rotation);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            LogIfProjectileHitCaster(other.transform.GetComponentInHierarchy<AbilityInvoker>());
            var target = base.GetTargetFromHierarchy(other.transform);
            base.HitTarget(target, -transform.forward);
        }
    }
}