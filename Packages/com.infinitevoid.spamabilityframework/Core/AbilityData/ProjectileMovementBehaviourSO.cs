using System;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using InfiniteVoid.SpamFramework.Core.Utils;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// To add your own custom movement behaviour, create a class that derives from this
    /// behaviour and assign it in the Ability's Projectile settings.
    /// </summary>
    [CreateAssetMenu(menuName = "SPAM Framework/Projectile movement", fileName = "movementBehaviour.asset")]
    public class ProjectileMovementBehaviourSO : ScriptableObject
    {
        [SerializeField] private ProjectileMovementType _movementType;
        [SerializeField] private ProjectileRotationTarget _desiredRotation;
        [SerializeField] private float _baseSpeed;

        [Header("Straight movement")]
        [Tooltip(
            "Check this if the projectile should be able target a point with another height than it's spawn point.")]
        [SerializeField]
        private bool _3dMovement;

        [Header("Curved movement")]
        [HelpBox("Curved movement can only be used with targeted projectiles")]
        [SerializeField]
        private CurvedMovementSettings _horizontalMovement;

        [SerializeField] private CurvedMovementSettings _verticalMovement;
        [SerializeField] private CurvedMovementSettings _speedCurve;


        private Vector3 _projectileStart;
        public bool MoveIn3D => _3dMovement;
        public ProjectileMovementType MovementType => _movementType;


        /// <summary>
        /// Sets the given position as projectile start position,
        /// and evaluates the strength of curves to apply for this particular instance
        /// </summary>
        /// <param name="projectileStartPos"></param>
        public void SetupMovement(Vector3 projectileStartPos)
        {
            _projectileStart = projectileStartPos;
            _horizontalMovement.CacheStrength();
            _verticalMovement.CacheStrength();
            _speedCurve.CacheStrength();
        }

        public Vector3 GetVelocity(Vector3 currentPosition, Vector3 targetPosition, bool is3dProjectile)
        {
            return _movementType switch
            {
                ProjectileMovementType.Straight => GetStraightVelocity(),
                ProjectileMovementType.AnimationCurve => GetAnimationCurvedVelocity(),
                ProjectileMovementType.AnimationCurve2d => GetAnimationCurvedVelocity(),
                _ => throw new NotSupportedException("The given MovementType is not supported")
            };

            Vector3 GetStraightVelocity()
            {
                var direction = (targetPosition - currentPosition).normalized;
                return direction * _baseSpeed;
            }

            Vector3 GetAnimationCurvedVelocity()
            {
                var distanceFromStart = 
                    is3dProjectile 
                        ? Distance.Between(_projectileStart, currentPosition.With(y: _projectileStart.y))
                        : Distance.Between(_projectileStart, currentPosition);
                var distanceStartToTarget =
                    is3dProjectile
                    ? Distance.Between(_projectileStart, targetPosition.With(y: _projectileStart.y))
                    : Distance.Between(_projectileStart, targetPosition);
                var percentargeAlongPath = distanceFromStart / distanceStartToTarget;
                var directionToTarget = (targetPosition - currentPosition);
                var horizNoise = Vector3.Cross(directionToTarget, Vector3.up).normalized;
                var noiseValue = _horizontalMovement.Evaluate(percentargeAlongPath);
                directionToTarget = directionToTarget.normalized;
                var movementOffset = new Vector3(
                    horizNoise.x * noiseValue,
                    directionToTarget.y + _verticalMovement.Evaluate(percentargeAlongPath),
                    horizNoise.z * noiseValue);
                var velocity = (directionToTarget * (_baseSpeed * _speedCurve.Evaluate(percentargeAlongPath))) +
                               movementOffset;
                return velocity;
            }
        }

        public Quaternion GetDesiredRotation(Transform projectileTransform, Vector3 curVelocity, Vector3 targetPos,
            Camera camera, bool is3dProjectile)
        {
            if (is3dProjectile)
                return Get3dRotation(projectileTransform, curVelocity, targetPos, camera);
            else
                return Get2dRotation(projectileTransform, curVelocity, targetPos, camera);
        }

        private Quaternion Get2dRotation(Transform projectileTransform, Vector3 curVelocity, Vector3 targetPos,
            Camera camera)
        {
            return _desiredRotation switch
            {
                ProjectileRotationTarget.CurrentDirection => Quaternion.LookRotation(curVelocity),
                ProjectileRotationTarget.Target => Quaternion.LookRotation(targetPos - projectileTransform.position),
                ProjectileRotationTarget.MainCamera => Quaternion.LookRotation(camera.transform.position - projectileTransform.position),
                ProjectileRotationTarget.DontRotate => projectileTransform.rotation,
                _ => throw new ArgumentOutOfRangeException(nameof(_desiredRotation))
            };
        }

        private Quaternion Get3dRotation(Transform projectileTransform, Vector3 curVelocity, Vector3 targetPos,
            Camera camera)
        {
            return _desiredRotation switch
            {
                ProjectileRotationTarget.CurrentDirection => Quaternion.LookRotation(curVelocity),
                ProjectileRotationTarget.Target => Quaternion.LookRotation(targetPos),
                ProjectileRotationTarget.MainCamera => Quaternion.LookRotation(camera.transform.position -
                                                                               projectileTransform.position),
                ProjectileRotationTarget.DontRotate => projectileTransform.rotation,
                _ => throw new ArgumentOutOfRangeException(nameof(_desiredRotation))
            };
        }
    }
}