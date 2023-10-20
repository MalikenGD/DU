using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PhysicsProjectile : Projectile
    {
        [SerializeField] private CircleTelegraph _telegraph;

        private Rigidbody _rby;
        private bool _usePhysics;
        private bool _telegraphed;
        private Collider _collider;
        private RigidbodyConstraints _initialConstraints;

        protected override bool Is3dProjectile => true;

        protected override void OnAwake()
        {
            TryGetComponent(out _collider);
            TryGetComponent(out _rby);
            if (_telegraph)
                _telegraph.transform.SetParent(null);
            _usePhysics = !_rby.isKinematic;
            _initialConstraints = _rby.constraints;
        }

        protected override void OnInit(ProjectileAbilitySO ability) { }

        protected override void OnResetProjectile()
        {
            this._usePhysics = !_rby.isKinematic;
            this._collider.enabled = true;
        }

        protected override void OnDeactivated()
        {
            if (_telegraph)
                _telegraph.Hide();
            _collider.enabled = false;
            _rby.velocity = _rby.angularVelocity = Vector3.zero;
            _rby.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected override void OnStopped()
        {
            if (_telegraph)
                _telegraph.Hide();
            _collider.enabled = false;
            _rby.velocity = _rby.angularVelocity = Vector3.zero;
            _rby.constraints = RigidbodyConstraints.FreezeAll;
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
            _rby.constraints = _initialConstraints;
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
                _rby.MoveRotation(desiredRotation);
            }
            else if (!_usePhysics && movementTiming == MovementTiming.Update)
            {
                Transform.position += velocity * Time.deltaTime;
                transform.rotation = desiredRotation;
            }

            if (_telegraph)
                _telegraph.SetPosition(targetPosition);
        }

        private void OnTriggerEnter(Collider other)
        {
            LogIfProjectileHitCaster(other.transform.GetComponentInHierarchy<AbilityInvoker>());
            var target = base.GetTargetFromHierarchy(other.transform);
            base.HitTarget(target, -transform.forward);
        }

        private void OnCollisionEnter(Collision collision)
        {
            LogIfProjectileHitCaster(collision.transform.GetComponentInHierarchy<AbilityInvoker>());
            var target = base.GetTargetFromHierarchy(collision.transform);
            HitTarget(target, collision.GetContact(0).normal);
        }
    }
}