using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// Base class for projectile abilities.
    /// </summary>
    public abstract class ProjectileAbility : AbilityBase
    {
        [Space(20)]
        [SerializeField] private ProjectilePool _projectilePool;
        
        [SerializeField] private Transform _spawnPoint;

        private ProjectileAbilitySO _currentAbility;

        protected override AbilityBaseSO Ability => _currentAbility;
        protected override bool CacheOnHitVfx => false;
        protected ProjectileAbilitySO ProjectileAbilitySo => _currentAbility;
        protected Vector3 SpawnPoint => _spawnPoint.position;

        protected Projectile GetProjectile(Vector3 position)
        {
            return _projectilePool.GetProjectile(position);
        }
        
        protected Projectile[] GetProjectiles(Vector3 position, int count)
        {
            var projectiles = new Projectile[count];
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i] = _projectilePool.GetProjectile(position, false);
            }

            return projectiles;
        }

        protected sealed override void OnStart()
        {
            _currentAbility =  _projectilePool.ProjectileAbilitySo;
            
            if (_projectilePool == null)
            {
                SpamLogger.LogError($"Pool is null in {gameObject.name}", this);
                return;
            }

            if (_currentAbility.Prefab is null)
            {
                SpamLogger.LogError($"Projectile is null in {_currentAbility.Name}", this);
                return;
            }
        }

        public void SetProjectileSpawnPoint(Transform spawnPoint)
        {
            this._spawnPoint = spawnPoint;
        }

        /// <summary>
        /// Sets the projectile-pool for this ability.
        /// </summary>
        /// <param name="pool"></param>
        public void SetPool(ProjectilePool pool)
        {
            this._projectilePool = pool;
        }

        protected override void CleanupOnDestroy()
        {
            if(_projectilePool)
                this._projectilePool.DestroySelf();
        }
    }
}