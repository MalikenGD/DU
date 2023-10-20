using System;
using System.Diagnostics;
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
        [SerializeField] private ProjectileAbilitySO _projectileAbility;
        [SerializeField] private ProjectilePool _projectilePool;
        [SerializeField] private Transform _spawnPoint;

        protected override AbilityBaseSO Ability => _projectileAbility;
        protected override bool CacheOnHitVfx => false;
        protected ProjectileAbilitySO ProjectileAbilitySo => _projectileAbility;
        protected Vector3 SpawnPoint => _spawnPoint.position;

        public ProjectilePool CurrentPool => _projectilePool;

        /// <summary>
        /// <para>
        /// Called right before a projectile is spawned and starts to move.
        /// If you require any other modification of the projectile pre-cast, add an action to this property and
        /// that action will run to modify the actual projectile instance that's about to spawn.
        /// </para>
        /// <para>
        /// Note that any changes you do might (or might not) break the projectile or get reset/overriden.
        /// If you're unsure, either check the source (<see cref="Projectile"/>) or ask for help on Discord.
        /// </para>
        /// <para>
        /// Currently known safe changes are:
        /// | Changing projectile layer
        /// | Setting/changing projectile tag
        /// </para>
        /// </summary>
        public Action<Projectile> PreCastProjectileSetup;

        /// <summary>
        /// Gets projectiles from the pool, spawned at <see cref="SpawnPoint"/>.
        /// The number of projectiles takes is the same as <paramref name="projectiles"/>.Length.
        /// This method does not allocate.
        /// </summary>
        /// <param name="projectiles">The array where projectiles will be stored</param>
        /// <returns></returns>
        protected void GetProjectiles(Projectile[] projectiles)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i] = _projectilePool.GetProjectile(SpawnPoint, false);
            }
        }

        /// <summary>
        /// Verifies that the projectiles array still has the same size as the projectile settings.
        /// This is only done in editor since number of projectiles spawned is not changeable in build.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected void EnsureCorrectArraySize(ref Projectile[] projectiles)
        {
            SpamLogger.EditorOnlyLog(projectiles.Length == 0, $"No projectiles to spawn for ability {this.name}. You must have at least one 'Spawned Projectile' for a Projectile Ability to fire projectiles", this.gameObject);
            if (ProjectileAbilitySo.SpawnedProjectiles.Count != projectiles.Length)
            {
                SpamLogger.LogDebug(SpamLogModules.ABILITIES, $"Projectile count changed for {ProjectileAbilitySo.Name}. Allocating new array size.");
                projectiles = new Projectile[ProjectileAbilitySo.SpawnedProjectiles.Count];
            }
        }
        protected override void OnStart()
        {
            SpamLogger.EditorOnlyErrorLog(!_projectileAbility, $"Ability is null in {gameObject.name}", this.gameObject);
            SpamLogger.EditorOnlyErrorLog(!_projectileAbility.Prefab, $"Projectile is null in {_projectileAbility.Name}", _projectileAbility);
#if UNITY_EDITOR
            if (!_projectileAbility || !_projectileAbility.Prefab) return;
#endif
            _projectilePool = SpamPoolManager.GetOrCreateProjectilePoolInScene(_projectileAbility);
            SpamLogger.EditorOnlyErrorLog(_projectilePool == null, $"Ability {_projectileAbility.Name} could not resolve it's pool", gameObject);
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

        public void SetAbility(ProjectileAbilitySO projectileAbilitySO)
        {
            this._projectileAbility = projectileAbilitySO;
        }
        protected override void CleanupOnDestroy()
        {
            if(_projectilePool)
                this._projectilePool.DestroySelf();
        }
        
#if UNITY_EDITOR
        /// <summary>
        ///  The ability this component represents. Only usable in editor
        /// </summary>
        public ProjectileAbilitySO CurrentAbility => _projectileAbility;
#endif
    }
}