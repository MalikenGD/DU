using System;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.VFX
{
    public class NullAbilityVFXHandler : IAbilityVFXHandler
    {
        public static NullAbilityVFXHandler Instance => _instance ??= new NullAbilityVFXHandler();
        private static NullAbilityVFXHandler _instance;

        private NullAbilityVFXHandler() => _instance = this;

        public void PlayWarmupVFX() { }
        public void TickOnHitVFX(float tickAmount) { }
        public void Update(float deltaTime) { }
        public bool VfxApplied => true;
        public void PlayOnHitVFX(ImpactPoint impactPoint, IAbilityTarget[] hitTargets) { }
        public void Destroy(Action<GameObject> destroyAction) { }
        public void StopVfx() { }
        public void PlayCastVfx(Vector3 transformForward) { }
        public void PlayImpactVfx(ImpactPoint impactPoint, Vector3? impactVfxSpawnPos, IAbilityTarget abilityTarget) { }
    }
}