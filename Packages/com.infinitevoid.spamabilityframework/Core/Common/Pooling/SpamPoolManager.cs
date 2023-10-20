using System;
using System.Collections.Generic;
using System.Diagnostics;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    /// <summary>
    /// Handles active pools at runtime.
    /// </summary>
    public class SpamPoolManager
    {
        private const int INITIAL_PROJECTILE_POOL_CAPACITY = 50;
        private const string LOG_MODULE = SpamLogModules.POOLING;

        /// <summary>
        /// All instantiated shared VFX Pools.
        /// Key is prefab hashcode
        /// </summary>
        private static Dictionary<int, ParticlePool> _sharedVfxPools = new Dictionary<int, ParticlePool>();

        private static bool _settingsInitialized = false;
        private static PoolingSettingsSO _poolSettings;

        /// <summary>
        /// All instantiated projectile pools.
        /// Key is projectile ability Id 
        /// </summary>
        private static Dictionary<int, ProjectilePool> _projectilePoolsInScene = new Dictionary<int, ProjectilePool>(INITIAL_PROJECTILE_POOL_CAPACITY);


        public static ParticlePool GetOrCreateParticlePool(GameObject prefab, int numInstances,
            Func<int, ParticlePool> createPoolFunc)
        {
            EnsureSettingsInitialized();
            int prefabHashCode = prefab.GetHashCode();
            var usesSharedPool = _poolSettings.VFXWithSharedPool.ContainsKey(prefabHashCode);
            if (!usesSharedPool)
                return createPoolFunc?.Invoke(numInstances);

            if (!_sharedVfxPools.TryGetValue(prefabHashCode, out var pool))
            {
                var numInstancesFromSettings = _poolSettings.VFXWithSharedPool[prefabHashCode];
                pool = createPoolFunc.Invoke(numInstancesFromSettings);
                _sharedVfxPools[prefabHashCode] = pool;
            }

            return pool;
        }

        /// <summary>
        /// Gets an existing or creates a projectile pool in the scene
        /// Used at runtime to make sure pools get reused properly.
        /// </summary>
        /// <param name="projectileAbilitySO"></param>
        /// <returns></returns>
        public static ProjectilePool GetOrCreateProjectilePoolInScene(ProjectileAbilitySO projectileAbilitySO)
        {
            EnsureSettingsInitialized();
            GetProjectilePoolsInScene();

            if (!_projectilePoolsInScene.TryGetValue(projectileAbilitySO.Id, out var pool))
            {
                SpamLogger.LogDebug(LOG_MODULE, $"No pool found in scene for {projectileAbilitySO.Name}, creating.");
                var poolObject = new GameObject();
                poolObject.name = $"{projectileAbilitySO.Name}_pool";
                // Only need to register the component here.
                // Pool will register itself on awake.InitPool by calling PoolManager.CreateProjectilePool
                pool = poolObject.AddComponent<ProjectilePool>();
                pool.SetProjectile(projectileAbilitySO);
                _projectilePoolsInScene.Add(projectileAbilitySO.Id, pool);
            }
            else
                SpamLogger.LogDebug(LOG_MODULE, $"Pool for {projectileAbilitySO.Name} found in scene, reusing.");

            return pool;
        }

        /// <summary>
        /// Creates a pool of projectiles by spawning projectiles or by getting
        /// all projectile children of the given component (when pre-filled)
        /// </summary>
        /// <param name="projectileAbility"></param>
        /// <param name="projectilePoolComponent"></param>
        /// <param name="onProjectileDeactivated"></param>
        /// <returns></returns>
        public static Pool<Projectile> CreateProjectilePool(ProjectileAbilitySO projectileAbility,
            ProjectilePool projectilePoolComponent, Action onProjectileDeactivated)
        {
            EnsureSettingsInitialized();
            var numToSpawn = _poolSettings.ProjectilePools[projectileAbility.Id];

            Pool<Projectile> pool = null;
            if (projectilePoolComponent.transform.childCount != 0)
            {
                var prespawnedProjectiles = projectilePoolComponent.GetComponentsInChildren<Projectile>(true);
                SpamLogger.LogDebug(LOG_MODULE, $"{projectilePoolComponent.gameObject.name} was prefilled. Loading {prespawnedProjectiles.Length} projectiles to pool...");
                for (int i = 0; i < prespawnedProjectiles.Length; i++)
                    prespawnedProjectiles[i].Init(projectileAbility, onProjectileDeactivated);
                pool = new Pool<Projectile>(prespawnedProjectiles);
            }
            else
            {
                SpamLogger.LogDebug(LOG_MODULE, $"{projectilePoolComponent.gameObject.name} was empty. Creating projectiles in pool for ability {projectileAbility.Name}...");
                pool = new Pool<Projectile>(
                    numToSpawn, 
                    () => Projectile.CreateInstance(projectileAbility, projectilePoolComponent.transform, onProjectileDeactivated));
            }

            return pool;
        }
        
        /// <summary>
        /// Removes the pool from the list of projectile pools.
        /// </summary>
        /// <param name="projectile"></param>
        public static void PoolDestroyed(ProjectileAbilitySO projectile, string poolName)
        {
            if (_projectilePoolsInScene.Count == 0) return;
            if(_projectilePoolsInScene.Remove(projectile.Id))
                SpamLogger.LogDebug(LOG_MODULE, $"Removing projectile pool {poolName}");
            else
                SpamLogger.EditorOnlyErrorLog(LOG_MODULE, $"{poolName} couldn't be found in Pool Manager, this might result in unexpected behaviour. Please report this as an error, and if possible include steps to reproduce.");

            if (!projectile.HasPooledVfx()) return;

            var vfxHashCode = projectile.VFX.OnHitVfx.GetHashCode();
            if (_sharedVfxPools.ContainsKey(vfxHashCode))
            {
                SpamLogger.LogDebug(LOG_MODULE, $"Removing shared VFX pool for {projectile.VFX.OnHitVfx.name}");
                _sharedVfxPools.Remove(vfxHashCode);
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void ClearObjectPoolOnSceneChange()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (_, __) =>
            {
                _sharedVfxPools.Clear();
                _projectilePoolsInScene = new Dictionary<int, ProjectilePool>(INITIAL_PROJECTILE_POOL_CAPACITY);
            };
        }
        
        private static void EnsureSettingsInitialized()
        {
            if (_settingsInitialized && _poolSettings) return;
            SpamLogger.LogDebug(LOG_MODULE, "Loaded pool settings from resources");
            _poolSettings = Resources.Load<PoolingSettingsSO>(SpamResources.POOL_SETTINGS);
            _settingsInitialized = true;
        }

        private static void GetProjectilePoolsInScene()
        {
            if (_projectilePoolsInScene.Count != 0) return;
            
            var pools = GameObject.FindObjectsOfType<ProjectilePool>();
            SpamLogger.LogDebug(LOG_MODULE, $"Getting pools in scene, found {pools.Length} pools.");
            for (int i = 0; i < pools.Length; i++)
                _projectilePoolsInScene.Add(pools[i].ProjectileAbilitySo.Id, pools[i]);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private void Reset()
        {
            _settingsInitialized = false;
            _sharedVfxPools = new Dictionary<int, ParticlePool>();
            _projectilePoolsInScene = new Dictionary<int, ProjectilePool>(INITIAL_PROJECTILE_POOL_CAPACITY);
        }

#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        [UnityEditor.InitializeOnLoadMethod]
        private static void ClearPoolingState()
        {
            UnityEditor.EditorApplication.playModeStateChanged += (state) =>
            {
                if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode &&
                    state != PlayModeStateChange.EnteredPlayMode) return;
                _sharedVfxPools.Clear();
                _projectilePoolsInScene = new Dictionary<int, ProjectilePool>(INITIAL_PROJECTILE_POOL_CAPACITY);
            };
            _settingsInitialized = false;
            _poolSettings = null;
            _projectilePoolsInScene = new Dictionary<int, ProjectilePool>(INITIAL_PROJECTILE_POOL_CAPACITY);
        }


        /// <summary>
        /// Gets the number of projectiles to be pooled for the given ability.
        /// Only usable in editor
        /// </summary>
        /// <param name="projectileAbilitySO"></param>
        /// <returns></returns>
        public static int GetNumProjectiles(ProjectileAbilitySO projectileAbilitySO)
        {
            EnsureSettingsInitialized();
            return _poolSettings.GetNumInstancesForProjectileAbility(projectileAbilitySO);
        }
        
        public static void RegisterPool(ProjectilePool projectilePool)
        {
            if (_projectilePoolsInScene.TryGetValue(projectilePool.ProjectileAbilitySo.Id, out _)) return;
            
            _projectilePoolsInScene.Add(projectilePool.ProjectileAbilitySo.Id, projectilePool);
        }
#endif
    }
}