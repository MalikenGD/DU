using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Physics2DProjectile : Projectile
    {
        [SerializeField] private CircleTelegraph _telegraph;

        private Rigidbody2D _rby;
        private bool _usePhysics;
        private bool _telegraphed;
        private Collider2D _collider;

        protected override bool Is3dProjectile => false;
        
        protected override void OnAwake()
        {
            TryGetComponent(out _collider);
            TryGetComponent(out _rby);
            if (_telegraph)
                _telegraph.transform.SetParent(null);
            _usePhysics = !_rby.isKinematic;
        }

        protected override void OnInit(ProjectileAbilitySO ability) { }

        protected override void OnResetProjectile()
        {
            this._usePhysics = _rby.simulated;
            this._collider.enabled = true;
        }

        protected override void OnDeactivated()
        {
            if (_telegraph)
                _telegraph.Hide();
            _collider.enabled = false;
            _rby.velocity = Vector2.zero;
            _rby.angularVelocity = 0;
        }

        protected override void OnStopped()
        {
            if (_telegraph)
                _telegraph.Hide();
            _rby.velocity = Vector2.zero;
            _rby.angularVelocity = 0;
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
            if (_usePhysics && movementTiming == MovementTiming.FixedUpdate)
            {
                _rby.velocity = velocity;
                _rby.SetRotation(desiredRotation);
            }
            else if (!_usePhysics && movementTiming == MovementTiming.Update)
            {
                _rby.MovePosition(Transform.position + velocity * Time.deltaTime);
                _rby.SetRotation(desiredRotation);
            }

            if (_telegraph)
                _telegraph.SetPosition(targetPosition);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            LogIfProjectileHitCaster(other.transform.GetComponentInHierarchy<AbilityInvoker>());
            var target = base.GetTargetFromHierarchy(other.transform);
            base.HitTarget(target, -transform.forward);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            LogIfProjectileHitCaster(collision.otherCollider.transform.GetComponentInHierarchy<AbilityInvoker>());
            var target = base.GetTargetFromHierarchy(collision.otherCollider.transform);
            HitTarget(target, collision.GetContact(0).normal);
        }
    }
}