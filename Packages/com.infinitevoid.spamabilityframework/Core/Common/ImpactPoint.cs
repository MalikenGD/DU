using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// A point in the world where an ability-impact happened.
    /// </summary>
    public struct ImpactPoint
    {
        /// <summary>
        /// The position of the impact
        /// </summary>
        public Vector3 Position { get; }
        /// <summary>
        /// The normal of the impact. If not using physics this usually defaults to the reversed direction of the projectile
        /// </summary>
        public Vector3 HitNormal { get; }

        public ImpactPoint(Vector3 pos)
        {
            this.Position = pos;
            this.HitNormal = Vector3.forward;
        }

        public ImpactPoint(Vector3 pos, Vector3 hitNormal)
        {
            this.Position = pos;
            this.HitNormal = hitNormal;
        }
    }
}