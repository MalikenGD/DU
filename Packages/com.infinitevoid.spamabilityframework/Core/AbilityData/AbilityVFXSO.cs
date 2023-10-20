using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for an ability's VFX (Visual Effects)
    /// </summary>
    [CreateAssetMenu(menuName = "SPAM Framework/Ability VFX", fileName = "abilityVfx.asset")]
    public class AbilityVFXSO : ScriptableObject
    {
        [Header("Warmup")]
        [SerializeField] private GameObject _warmupVfx;
        [SerializeField] private float _warmupLifeTime = 1;
        [SerializeField] private Vector3 _warmupCustomScale = Vector3.one;
        
        [Header("Cast")]
        [SerializeField] private GameObject _castVfx;
        [SerializeField] private CastVfxRotationEnum _castRotation;
        [SerializeField] private Vector3 _castCustomScale = Vector3.one;
        [SerializeField] private Vector3 _castFixedRotation = Vector3.zero;
        
        [Header("Impact")]
        [SerializeField] private GameObject _impactVfx;
        [SerializeField] private ImpactVfxRotationEnum _impactRotation;
        [SerializeField] private ImpactVfxSpawnSettingsEnum _impactSpawnPosition;
        [SerializeField] private Vector3 _impactCustomScale = Vector3.one;
        [SerializeField] private Vector3 _impactFixedRotation = Vector3.zero;
        [SerializeField] private LayerMask _impactSpawnGroundLayers;
        [Tooltip("The offset from the impact point to apply when spawning impact VFX. Useful when the impact needs to be offset to be visible, f.eg. when impact should spawn on the ground.")]
        [SerializeField] private Vector3 _impactSpawnOffset = Vector3.zero;
        
 
        [Header("On Hit")]
        [SerializeField] private GameObject _onHitVfx;
        [SerializeField] private OnHitParticleCountEnum _numOfOnHitInstances;
        [SerializeField] private int _onHitInstances = 0;
        [SerializeField] private OnHitSpawnPointEnum _onHitSpawnPoint;
        [SerializeField] private Vector3 _onHitPositionOffset = Vector3.zero;
        [SerializeField] private OnHitRotationEnum _onHitRotation = OnHitRotationEnum.Initial;
        [SerializeField] private bool _spawnOnHitAtTargetBase;
        
        [SerializeField] private TimeHandlingEnum _lifeTimeHandling;
        [SerializeField] private float _onHitLifeTime = 3;
        
        public GameObject WarmupVfx => _warmupVfx;
        public float WarmupLifeTime => _warmupLifeTime;
        public GameObject OnHitVfx => _onHitVfx;
        public Vector3 OnHitPositionOffset => _onHitPositionOffset;
        public float OnHitLifeTime => _onHitLifeTime;
        public bool SpawnOnHitAtTargetBase => _spawnOnHitAtTargetBase;
        public Vector3 WarmupCustomScale => _warmupCustomScale;
        public bool AutomaticOnHitLifetime => _lifeTimeHandling == TimeHandlingEnum.Automatic;
        public int OnHitInstances => _onHitInstances;
        public bool CustomOnHitInstances => _numOfOnHitInstances == OnHitParticleCountEnum.Custom;
        public bool SpawnOnHitAtImpactPoint => _onHitSpawnPoint == OnHitSpawnPointEnum.ImpactPoint;
        public bool SameOnHitCountAsMaxTargets => _numOfOnHitInstances == OnHitParticleCountEnum.SameAsMaxTargets;
        public bool OnHitUseSharedPool => _onHitVfx && _numOfOnHitInstances == OnHitParticleCountEnum.SharedPool;
        public bool RotateOnHitToNormal => _onHitRotation == OnHitRotationEnum.TargetNormal;
        public GameObject CastVfx => _castVfx;
        public bool RotateCastVfx => _castRotation != CastVfxRotationEnum.Initial;
        public Vector3 CastVfxCustomScale => _castCustomScale;

        public GameObject ImpactVfx => _impactVfx;
        public bool RotateImpactVfx => _impactRotation != ImpactVfxRotationEnum.Initial;
        public Vector3 ImpactCustomScale => _impactCustomScale;
        public bool SpawnImpactOnFixedPoint => _impactSpawnPosition == ImpactVfxSpawnSettingsEnum.FixedPosition;
        public float GetOnHitLifetime() => AutomaticOnHitLifetime ? 0 : OnHitLifeTime;

        public Vector3 GetOnHitRotation(Vector3 impactPointNormal) => RotateOnHitToNormal ? impactPointNormal : OnHitVfx.transform.forward;
        
        public Quaternion GetCastVfxRotation(Vector3 casterForward)
        {
            Assert.IsTrue(this.RotateCastVfx, "GetCastRotation should not be called when rotate cast VFX is false");
            if (_castRotation == CastVfxRotationEnum.CasterForward)
                return Quaternion.LookRotation(casterForward);

            return Quaternion.Euler(_castFixedRotation);
        }
        
        public Quaternion GetImpactVfxRotation(Vector3 surfaceNormal)
        {
            Assert.IsTrue(this.RotateImpactVfx, "GetImpactRotation should not be called when rotate cast VFX is false");
            if (_impactRotation == ImpactVfxRotationEnum.ImpactPointNormal)
                return Quaternion.LookRotation(surfaceNormal);
            
            return Quaternion.Euler(_impactFixedRotation);
        }
        
        
        // stored values for no GC
        private Ray _ray;
        private RaycastHit[] _hits = new RaycastHit[1];
        public Vector3 GetImpactVfx3dPosition(Vector3 fixedPosition, Vector3 impactPointPosition,
            IAbilityTarget abilityTarget)
        {
            switch (_impactSpawnPosition)
            {
                case ImpactVfxSpawnSettingsEnum.FixedPosition:
                    return fixedPosition;
                case ImpactVfxSpawnSettingsEnum.ImpactPoint:
                    return impactPointPosition + _impactSpawnOffset;
                case ImpactVfxSpawnSettingsEnum.GroundOfImpact:
                case ImpactVfxSpawnSettingsEnum.GroundOfTargetThenImpact:
                    Vector3 rayOrigin;
                    if (_impactSpawnPosition == ImpactVfxSpawnSettingsEnum.GroundOfTargetThenImpact)
                        rayOrigin = abilityTarget != null
                            ? abilityTarget.Transform.position
                            : impactPointPosition;
                    else rayOrigin = impactPointPosition;
                    
                    _ray = new Ray(rayOrigin.Add(y: 1), Vector3.down);
                    var numHits = Physics.RaycastNonAlloc(_ray, _hits, 10f, _impactSpawnGroundLayers);
                    if (numHits != 0) return _hits[0].point + _impactSpawnOffset;
                    
                    SpamLogger.LogWarning($"Could not find ground layer to spawn impact on, impact will spawn on impact point. Did you set the correct ground-layers in impact VFX settings for {this.name}?");
                    return impactPointPosition;

                default:
                    return impactPointPosition;
            }
        }

        private enum OnHitSpawnPointEnum
        {
            Targets,
            ImpactPoint
        }

        private enum OnHitParticleCountEnum
        {
            SameAsMaxTargets,
            Custom,
            SharedPool
        }
        private enum OnHitRotationEnum
        {
            Initial,
            TargetNormal
        }
    }
}