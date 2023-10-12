using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Audio;
using InfiniteVoid.SpamFramework.Core.Common.VFX;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using InfiniteVoid.SpamFramework.Core.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Projectiles
{
    /// <summary>
    /// Base for all projectiles. Be mindful you dont accidentally declare any unity methods in your
    /// deriving class that this class already implements.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject _projectileVisual;

        /// <summary>
        /// Here you implement your own Awake logic
        /// </summary>
        protected abstract void OnAwake();

        /// <summary>
        /// Here you implement your own Init logic. Init is called on each projectile when it's created and added to a pool.
        /// </summary>
        /// <param name="ability"></param>
        protected abstract void OnInit(ProjectileAbilitySO ability);

        /// <summary>
        /// Here you implement your own reset logic
        /// </summary>
        protected abstract void OnResetProjectile();

        /// <summary>
        /// Here you implement your own deactivate logic.
        /// This is called if the projectile is set to deactivate on hit
        /// </summary>
        protected abstract void OnDeactivated();

        /// <summary>
        /// Here you implement your own stop logic.
        /// This is called if the projectile is set to stop on hit
        /// </summary>
        protected abstract void OnStopped();

        /// <summary>
        /// Here you implement your own OnDestroy logic 
        /// </summary>
        protected abstract void OnDestroyed();

        /// <summary>
        /// Called just before the projectile starts moving.
        /// </summary>
        /// <param name="projectileInvoker">The invoker of the projectile</param>
        /// <param name="projectileSo">The projectile ability data</param>
        /// <param name="target">The projectile target position</param>
        /// <param name="settings">The projectile instance settings.</param>
        /// <param name="shouldBeTelegraphed">If this is not null, an explicit value has been set for if the projectile should be telegraphed</param>
        protected abstract void OnSetupForMove(AbilityInvoker projectileInvoker, ProjectileAbilitySO projectileSo,
            Vector3 target, ProjectileSettings settings, bool? shouldBeTelegraphed);

        /// <summary>
        /// Here you implement the actual moving of the projectile. See examples in f.eg. <see cref="RigidbodyProjectile"/>.
        /// This will either be called in <see cref="Update"/> or <see cref="FixedUpdate"/>, which is described by <paramref name="movementTiming"/>
        /// </summary>
        /// <param name="velocity">The current velocity of the projectile. This includes speed</param>
        /// <param name="targetPoint">The target point, this is where the projectile ultimately is headed</param>
        /// <param name="desiredRotation"></param>
        /// <param name="movementTiming">Holds if the call was made from Update or FixedUpdate</param>
        protected abstract void MoveProjectile(Vector3 velocity, Vector3 targetPoint, Quaternion desiredRotation,
            MovementTiming movementTiming);

        protected abstract bool Is3dProjectile { get; }

        protected enum MovementTiming
        {
            Update,
            FixedUpdate
        }

        [CanBeNull] protected IAbilityTarget Target => _target;

        /// <summary>
        /// Cached transform
        /// </summary>
        protected Transform Transform;

        private AbilityInvoker _projectileInvoker;
        private AbilityEffectsApplier _abilityEffectsApplier;
        private AbilitySFXHandler _abilitySfxHandler;
        private AbilityTargetsResolver _targetResolver;
        private AudioSource _audioSource;
        private IAbilityVFXHandler _abilityVfxHandler;
        private event Action Deactivated;
        /// <summary>
        /// This will be set to true if it's a targeted projectile cast on ground.
        /// In case of a directional projectile, this will be false.
        /// </summary>
        private bool _destroyOnTargetPoint;
        private Vector3 _targetPoint;
        private float _distanceCheck;
        private bool _hasLifetime;
        private float _deactivationTimer;
        private float _onHitClipLength;
        private bool _stopOnHit;
        private bool _shouldMove;
        private ProjectileMovementBehaviourSO _movementBehaviour;
        private float _spawnDelay;
        private Action<GameObject> _destroyAction;
        private bool DelayedStart => 0 < _spawnDelay;
        private float _timeToLive;
        private bool _deactivateOnHit;
        private Vector3 _velocity;
        private IAbilityTarget _target;
        private Camera _camera;


        /// <summary>
        /// Initialises the projectile. Call this after instantiating a new projectile or when changing the underlying
        /// ability of the projectile (f.eg when upgrading without changing visuals)
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="parent"></param>
        /// <param name="onDeactivate"></param>
        public void Init(ProjectileAbilitySO ability, Transform parent = null, Action onDeactivate = null)
        {
            var conditionConstraintsValidator = new ConditionConstraintsValidator(ability);
            _abilityEffectsApplier = new AbilityEffectsApplier(ability, conditionConstraintsValidator);
            _abilitySfxHandler = new AbilitySFXHandler(_audioSource, ability.SFX);
            _targetResolver = new AbilityTargetsResolver(ability, conditionConstraintsValidator);
            this.Deactivated += onDeactivate;
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            // If this comparison is not made then it will evaluate to true even if no VFX is set
            if (ability.VFX == null)
            {
                _abilityVfxHandler = NullAbilityVFXHandler.Instance;
            }
            else
            {
                _abilityVfxHandler =
                    new AbilityVFXHandler(ability,
                        null,
                        getOnHitVfxInstance: () =>
                        {
                            var vfxTransform =
                                Instantiate(ability.VFX.OnHitVfx, transform.position, Quaternion.identity).transform;
                            if (parent)
                                vfxTransform.SetParent(parent);
                            return vfxTransform;
                        });
            }

            OnInit(ability);
        }


        public void MoveInDirection(Vector3 targetDirection,
            ProjectileAbilitySO projectileSo,
            AbilityInvoker projectileInvoker, ProjectileSettings settings)
        {
            SetupForMove(projectileInvoker, projectileSo, targetDirection, settings, false);
            _destroyOnTargetPoint = false;
        }

        public void MoveTo(Vector3 point,
            ProjectileAbilitySO projectileSo,
            AbilityInvoker projectileInvoker,
            ProjectileSettings settings,
            float distanceCheck = .1f)
        {
            SetupForMove(projectileInvoker, projectileSo, point, settings);
            _destroyOnTargetPoint = true;
            _distanceCheck = distanceCheck;
        }

        public void MoveToTarget(IAbilityTarget target,
            ProjectileAbilitySO projectileSo,
            AbilityInvoker projectileInvoker, ProjectileSettings settings)
        {
            var targetPos = target.Transform.position;
            SetupForMove(projectileInvoker, projectileSo, targetPos, settings);
            this._target = target;
        }

        private void ResetProjectile()
        {
            this.gameObject.SetActive(true);
            this._destroyOnTargetPoint = false;
            this._target = null;
            this.enabled = true;
            this._shouldMove = true;
            this._projectileVisual.SetActive(true);
            this._deactivationTimer = 0;
            if (_audioSource.isPlaying)
                _audioSource.Stop();
            OnResetProjectile();
        }

        private void Awake()
        {
            TryGetComponent(out _audioSource);
            Transform = transform;
            _audioSource.loop = true;
            _destroyAction = (go) => Destroy(go);
            _camera = Camera.main;
            OnAwake();
            SpamLogger.EditorOnlyErrorLog(this.gameObject == _projectileVisual.gameObject, $"Projectile visual is same as the projectile root object. This will prevent the projectile from applying effects. Please select a child GameObject as visual for {this.name}");
        }

        private void Update()
        {
            if (DelayedStart)
            {
                _spawnDelay -= Time.deltaTime;
                if (_spawnDelay <= 0 && !_projectileVisual.activeSelf)
                {
                    _projectileVisual.SetActive(true);
                    _audioSource.Play();
                }
                return;
            }

            if (0 < _deactivationTimer)
            {
                float deltatime = Time.deltaTime;
                _abilityEffectsApplier.Update(deltatime);
                _deactivationTimer -= deltatime;
                if (_deactivationTimer <= 0)
                {
                    this.Deactivated?.Invoke();
                    this.gameObject.SetActive(false);
                }
            }

            if (!_shouldMove) return;
            if (_target != null)
            {
                if (!_target.Transform)
                {
                    Deactivate();
                    return;
                }

                _targetPoint = _target.Transform.position;
            }

            _velocity = _destroyOnTargetPoint 
                ? _movementBehaviour.GetVelocity(Transform.position, _targetPoint, Is3dProjectile)
                : _velocity;
            
            if (_hasLifetime)
            {
                _timeToLive -= Time.deltaTime;
                if (_timeToLive <= 0)
                {
                    Deactivate();
                    return;
                }
            }

            if (_destroyOnTargetPoint)
            {
                if (Distance.IsLessThan(_targetPoint, Transform.position, _distanceCheck))
                {
                    HitTarget(null, Vector3.up);
                    return;
                }
            }

            MoveProjectile(_velocity, _targetPoint, GetRotation(), MovementTiming.Update);
        }

        private void FixedUpdate()
        {
            if (!_shouldMove) return;

            MoveProjectile(_velocity, _targetPoint, GetRotation(), MovementTiming.FixedUpdate);
        }

        private Quaternion GetRotation() =>
            _movementBehaviour.GetDesiredRotation(Transform, _velocity, _targetPoint, _camera, Is3dProjectile);

        private void SetFieldsFromProjectileData(ProjectileAbilitySO projectileSo,
            ProjectileSettings projectileSettings)
        {
            this._deactivateOnHit = projectileSo.DeactivateOnHit;
            this._stopOnHit = projectileSo.StopOnHit;
            this._timeToLive = projectileSo.TimeToLive;
            this._hasLifetime = 0 < projectileSo.TimeToLive;
            this._movementBehaviour = projectileSettings.MovementBehaviour;
            this._spawnDelay = projectileSettings.SpawnTime.GetValue();
        }

        private void SetupForMove(AbilityInvoker projectileInvoker, ProjectileAbilitySO projectileSo, Vector3 targetDirection,
            ProjectileSettings settings, bool? telegraphedOverride = null)
        {
#if UNITY_EDITOR
            if (settings.MovementBehaviour is null)
                throw new InvalidOperationException(
                    $"Movement hasn't been set for a projectile in ability: {projectileSo.name}");
#endif
            ResetProjectile();
            SetFieldsFromProjectileData(projectileSo, settings);
            this._targetPoint = settings.MovementBehaviour.MoveIn3D ?  targetDirection : targetDirection.With(y: transform.position.y);
            _projectileInvoker = projectileInvoker;
            var startPos = Transform.position;
            _movementBehaviour.SetupMovement(startPos);
            _velocity = _movementBehaviour.GetVelocity(startPos, targetDirection, Is3dProjectile);
            if (DelayedStart)
                _projectileVisual.SetActive(false);
            PlayInFlightSound(projectileSo);
            OnSetupForMove(projectileInvoker, projectileSo, targetDirection, settings, telegraphedOverride);
        }


        private void PlayInFlightSound(ProjectileAbilitySO projectileSo)
        {
            if (!projectileSo.InFlightSound) return;

            _audioSource.clip = projectileSo.InFlightSound;
            if (DelayedStart) return;
            _audioSource.Play();
        }

        /// <summary>
        /// Make the projectile hit the given target, or targets in its range.
        /// Call this when the projectile successfully hits a target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="normal"></param>
        protected void HitTarget([CanBeNull] IAbilityTarget target, Vector3 normal)
        {
            if (0 < _deactivationTimer) return;
            var impactPoint = new ImpactPoint(Transform.position, normal);
            _targetResolver.ResolveTargets(_projectileInvoker, impactPoint.Position, target);
            SpamLogger.LogDebug("[Projectile]",
                $"Projectile {Transform.name} hit at point {impactPoint.Position.ToString()} with target {target?.Transform.name}.",
                this);
            _abilityVfxHandler?.ExecuteOnHitVFX(impactPoint, _targetResolver.ValidTargets);
            _abilityEffectsApplier.ApplyEffects(impactPoint.Position, _targetResolver.ValidTargets, _projectileInvoker);
            _onHitClipLength = _abilitySfxHandler.PlayOnHitSfx(null);

            if (_deactivateOnHit)
                Deactivate();
            else if (_stopOnHit)
            {
                _shouldMove = false;
                OnStopped();
            }
        }

        private void Deactivate()
        {
            _projectileVisual.SetActive(false);
            _shouldMove = false;
            // Add extra offset to make sure effects or sound isn't cut off 
            float extraDeactivationTimeOffset = .2f;
            _deactivationTimer = Mathf.Max(_onHitClipLength, _abilityEffectsApplier.MaxEffectTime) +
                                 extraDeactivationTimeOffset;
            OnDeactivated();
        }

        private void OnDestroy()
        {
            if (_abilityVfxHandler != null)
                _abilityVfxHandler.Destroy(_destroyAction);
            Deactivated = null;
            OnDestroyed();
        }
    }
}