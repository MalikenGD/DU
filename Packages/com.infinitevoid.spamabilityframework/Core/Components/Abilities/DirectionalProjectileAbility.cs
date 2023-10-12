using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of
    /// an <see cref="ProjectileAbilitySO"/> in a given direction
    /// </summary>
    public class DirectionalProjectileAbility : ProjectileAbility
    {
#if UNITY_EDITOR
        [SerializeField] private bool _drawCastPoint;
        [SerializeField] private Color _gizmoColor = Color.red;
#endif
        private Vector3 _pointInDirection;
        private Projectile[] _projectiles;

        public override void Cast(Vector3 pointInDirection)
        {
            _pointInDirection = pointInDirection;
            base.CastAbility(_pointInDirection);
        }

        protected override void OnCast()
        {
            base.PlayCastSFX();
            _projectiles = base.GetProjectiles(base.SpawnPoint, base.ProjectileAbilitySo.SpawnedProjectiles.Count);
            SpamLogger.EditorOnlyLog(_projectiles.Length == 0, $"No projectiles to spawn for ability {this.name}. Did you add projectiles to the ability?", this.gameObject);
            for (int i = 0; i < _projectiles.Length; i++)
            {
                _projectiles[i].MoveInDirection(_pointInDirection, base.ProjectileAbilitySo, base.Invoker, base.ProjectileAbilitySo.SpawnedProjectiles[i]);
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_drawCastPoint) return;
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(_pointInDirection, .5f);
        }
#endif
    }
}