using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Utils;

namespace InfiniteVoid.SpamFramework.Core.Projectiles
{
    /// <summary>
    /// An object representing a Projectile and it's settings, i.e. spawn time, movement behaviour etc.
    /// </summary>
    [Serializable]
    public class ProjectileSettings
    {
        public FloatValue SpawnTime;
        public ProjectileMovementBehaviourSO MovementBehaviour;
    }
}