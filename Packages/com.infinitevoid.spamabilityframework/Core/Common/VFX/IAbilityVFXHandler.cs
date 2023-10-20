using System;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using JetBrains.Annotations;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.VFX
{
    public interface IAbilityVFXHandler
    {
        void PlayWarmupVFX();
        /// <summary>
        /// Ticks the OnHitVFX lifetime. Does nothing if the OnHitVfx has automatic lifetime management.
        /// </summary>
        /// <param name="tickAmount"></param>
        void TickOnHitVFX(float tickAmount);
        void Update(float deltaTime);
        bool VfxApplied { get;}
        void PlayOnHitVFX(ImpactPoint impactPoint, IAbilityTarget[] hitTargets);
        void Destroy(Action<GameObject> destroyAction);
        void StopVfx();
        void PlayCastVfx(Vector3 casterForward);
        void PlayImpactVfx(ImpactPoint impactPoint, Vector3? impactVfxSpawnPos, [CanBeNull] IAbilityTarget target);
    }
}