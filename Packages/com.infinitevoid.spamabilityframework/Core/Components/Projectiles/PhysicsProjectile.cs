using InfiniteVoid.SpamFramework.Core.AbilityData;
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

        protected override bool Is3dProjectile => true;
        
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
            this._usePhysics = !_rby.isKinematic;
            this._collider.enabled = true;
        }

        protected override void OnDeactivated()
        {
            if (_telegraph)
                _telegraph.Hide();
        }

        protected override void OnStopped() { }

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
            _collider.enabled = false;
            base.HitTarget(other.GetComponent<IAbilityTarget>(), -transform.forward);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            _collider.enabled = false;
            HitTarget(collision.collider.GetComponent<IAbilityTarget>(), collision.GetContact(0).normal);
        }
    }
}