using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.VFX
{
    public class AbilityVFXHandler : IAbilityVFXHandler
    {
        private readonly AbilityVFXSO _abilityVfx;
        private GameObject _warmupVfx;
        private GameObject _castVfx;
        private GameObject _impactVfx;
        private ParticlePool _onHitVfxPool;
        private float _warmupLifetime;


        public bool VfxApplied => (!_warmupVfx || !_warmupVfx.activeSelf) &&
                                  (_onHitVfxPool == null || _onHitVfxPool.AllParticlesInactive);

        private const string LOG_MODULE = SpamLogModules.ABILITY_VFX;
        public AbilityVFXHandler(AbilityBaseSO ability,
            Func<GameObject> getWarmupVfxInstance,
            Func<Transform> getOnHitVfxInstance,
            Func<GameObject> getCastVfxInstance,
            Func<GameObject> getImpactVfxInstance)
        {
            if (!ability.VFX) return;
            _abilityVfx = ability.VFX;

            CacheVFX(getWarmupVfxInstance, getOnHitVfxInstance, getCastVfxInstance, getImpactVfxInstance);
        }

        public void PlayWarmupVFX()
        {
            if (!_warmupVfx) return;

            _warmupVfx.SetActive(true);
            _warmupLifetime = _abilityVfx.WarmupLifeTime;
        }

        public void PlayOnHitVFX(ImpactPoint impactPoint, IAbilityTarget[] hitTargets)
        {
            if (!_abilityVfx.OnHitVfx) return;
            var lifeTime = _abilityVfx.GetOnHitLifetime();
            var vfxForward = _abilityVfx.GetOnHitRotation(impactPoint.HitNormal);
            if (_abilityVfx.SpawnOnHitAtImpactPoint)
            {
                _onHitVfxPool.SpawnParticleAt(impactPoint.Position + _abilityVfx.OnHitPositionOffset, vfxForward);
                return;
            }

            for (var i = 0; i < hitTargets.Length; i++)
            {
                if (hitTargets[i] == null) return;

                if (_abilityVfx.SpawnOnHitAtTargetBase)
                    _onHitVfxPool.SpawnParticleAt(hitTargets[i].BasePosition + _abilityVfx.OnHitPositionOffset, vfxForward,
                        lifeTime);
                else
                    _onHitVfxPool.SpawnParticleAt(hitTargets[i].Transform.position + _abilityVfx.OnHitPositionOffset, vfxForward,
                        lifeTime);
            }
        }

        public void Destroy(Action<GameObject> destroyAction)
        {
            _onHitVfxPool?.Clear(destroyAction);
            _onHitVfxPool = null;
            if (_warmupVfx)
                destroyAction(_warmupVfx);
            if (_castVfx)
                destroyAction(_castVfx);
        }

        public void StopVfx()
        {
            if(_warmupVfx)
                _warmupVfx.SetActive(false);
            if(_castVfx)
                _castVfx.SetActive(false);
        }

        public void PlayCastVfx(Vector3 casterForward)
        {
            SpamLogger.LogDebug(_castVfx, LOG_MODULE, "Play cast VFX");
            SpamLogger.LogDebug(!_castVfx, LOG_MODULE, "No cast VFX, returning");
            if (!_castVfx) return;
            _castVfx.SetActive(false);
            if (!_abilityVfx.RotateCastVfx)
            {
                _castVfx.SetActive(true);
                return;
            }

            _castVfx.transform.rotation = _abilityVfx.GetCastVfxRotation(casterForward);
            _castVfx.SetActive(true);
        }

        public void PlayImpactVfx(ImpactPoint impactPoint, Vector3? impactVfxSpawnPos, IAbilityTarget abilityTarget)
        {
            SpamLogger.LogDebug(_impactVfx, LOG_MODULE, "Play impact VFX");
            SpamLogger.LogDebug(!_impactVfx, LOG_MODULE, "No impact VFX, returning");
            if (!_impactVfx) return;
            
            _impactVfx.SetActive(false);
            var impactVfxTransform = _impactVfx.transform;
            var impactRotation = impactVfxTransform.rotation;
            if(_abilityVfx.RotateImpactVfx)
                impactRotation = _abilityVfx.GetImpactVfxRotation(impactPoint.HitNormal);

            if(_abilityVfx.SpawnImpactOnFixedPoint && !impactVfxSpawnPos.HasValue)
                SpamLogger.LogError("Impact VFX has no spawn point set.");
                
            var pos = _abilityVfx.GetImpactVfx3dPosition(impactVfxSpawnPos ?? impactVfxTransform.position, impactPoint.Position, abilityTarget);
            impactVfxTransform.SetPositionAndRotation(pos, impactRotation);
            _impactVfx.SetActive(true);
        }

        /// <summary>
        /// Ticks the OnHitVFX lifetime. Does nothing if the OnHitVfx has automatic lifetime management.
        /// </summary>
        /// <param name="tickAmount"></param>
        public void TickOnHitVFX(float tickAmount)
        {
            if (!_abilityVfx.OnHitVfx || _abilityVfx.AutomaticOnHitLifetime) return;
            _onHitVfxPool?.TickLifetimes(tickAmount);
        }

        public void Update(float deltaTime)
        {
            _warmupLifetime -= deltaTime;
            if (_warmupVfx && _warmupLifetime <= 0)
                _warmupVfx.SetActive(false);

            if (!_abilityVfx.AutomaticOnHitLifetime || !_abilityVfx.OnHitVfx) return;
            _onHitVfxPool?.TickLifetimes(deltaTime);
        }


        private void CacheVFX(Func<GameObject> getWarmupVfxInstance, Func<Transform> getOnHitVfxInstance,
            Func<GameObject> getCastVfxInstance, Func<GameObject> getImpactVfxInstance)
        {
            if (_abilityVfx.WarmupVfx && getWarmupVfxInstance != null)
            {
                _warmupVfx = getWarmupVfxInstance.Invoke();
                _warmupVfx.transform.localScale = _abilityVfx.WarmupCustomScale;
                _warmupVfx.SetActive(false);
            }

            if (_abilityVfx.OnHitVfx && getOnHitVfxInstance != null)
            {
                _onHitVfxPool = SpamPoolManager.GetOrCreateParticlePool(_abilityVfx.OnHitVfx, _abilityVfx.OnHitInstances, (numInstances) => new ParticlePool(numInstances, getOnHitVfxInstance));
            }

            if (_abilityVfx.CastVfx && getCastVfxInstance != null)
            {
                _castVfx = getCastVfxInstance.Invoke();
                _castVfx.transform.localScale = _abilityVfx.CastVfxCustomScale;
                _castVfx.SetActive(false);
            }

            if (_abilityVfx.ImpactVfx && getImpactVfxInstance != null)
            {
                _impactVfx = getImpactVfxInstance.Invoke();
                _impactVfx.transform.localScale = _abilityVfx.ImpactCustomScale;
                _impactVfx.SetActive(false);
            }
        }
    }
}