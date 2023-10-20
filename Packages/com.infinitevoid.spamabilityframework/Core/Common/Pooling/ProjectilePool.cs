using System;
using System.Diagnostics;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
// Until obsolete _numInPool is removed, disable the warnings
#pragma warning disable CS0618

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    /// <summary>
    /// A specialized pool for handling projectiles 
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [FormerlySerializedAs("_projectileInPool")] [SerializeField]
        private ProjectileAbilitySO _projectileAbility;
        
        [SerializeField, FormerlySerializedAs("_noInPool"), Obsolete("This value is no longer used, but instead gotten from the pooling window. This property will be removed in a future version.")] 
        [Tooltip("This property is obsolete. The number in pool is now configured in the Pooling Window.")]
        private int _numInPool = 1;

        // [FormerlySerializedAs("_noInPool")] [SerializeField] private int _numInPool = 1;
        [HelpBox(
            "When abilities that reference this pool are destroyed they call Destroy on this pool. Check this if you want to manage its lifetime manually.")]
        [SerializeField]
        private bool _keepAlive;

        private Pool<Projectile> _projectilePool;
        private Transform _transform;
        private int _numProjectilesAlive = 0;
        private Action _decreaseNumProjectileAliveAction;

        public ProjectileAbilitySO ProjectileAbilitySo => _projectileAbility;

        private void Awake()
        {
            _transform = this.transform;
            _decreaseNumProjectileAliveAction = () => this._numProjectilesAlive--;
            SpamLogger.LogWarning(this.transform.parent, $"{this.name} is not located in the root of the scene. This could lead to undefined behaviour. Please move the pool to the root of the scene.", this);
        }

        private void Start()
        {
            SpamLogger.EditorOnlyErrorLog(!_projectileAbility, $"No projectile ability assigned to pool: {this.gameObject.name}", this.gameObject);
            SpamLogger.EditorOnlyErrorLog(!_projectileAbility.Prefab, $"No projectile assigned to {_projectileAbility.Name}", _projectileAbility);
#if UNITY_EDITOR
            if (!_projectileAbility || !_projectileAbility.Prefab) return;
#endif
            InitPool();
            this.enabled = false;
        }

        /// <summary>
        /// Pool component gets enabled when the pool should be destroyed.
        /// </summary>
        private void Update()
        {
            if (0 < this._numProjectilesAlive)
                return;

            Destroy(this.gameObject);
        }

        private void InitPool()
        {
            if (!this._transform)
                this._transform = transform;

            #if UNITY_EDITOR
            VerifyCorrectNumberOfProjectiles();
            #endif
            _projectilePool =
                SpamPoolManager.CreateProjectilePool(_projectileAbility, this, _decreaseNumProjectileAliveAction);
        }

        /// <summary>
        /// Sets the ability to use in the pool.
        /// </summary>
        /// <param name="projectileAbility"></param>
        public void SetProjectile(ProjectileAbilitySO projectileAbility) => _projectileAbility = projectileAbility;

        public Projectile GetProjectile(Vector3 position, bool active = true)
        {
            _numProjectilesAlive++;
            return _projectilePool.GetNext(position, active, Vector3.forward);
        }

        public void DestroySelf()
        {
            if (_keepAlive)
                return;
            if (this)
            {
                this.enabled = true;
            }
        }

        private void OnDestroy()
        {
#if SPAM_DEBUG
            SpamLogger.LogDebug(SpamLogModules.POOLING, $"{this.gameObject.name} destroyed");
#endif
            SpamPoolManager.PoolDestroyed(this._projectileAbility, this.name);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Refills the pool with the new number of projectiles that should be in it depending on config settings.
        /// Only usable in editor.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public void RefillPool()
        {
            SpamLogger.LogWarning(EditorApplication.isPlaying,
                $"{this.name} was refilled in playmode, this is a costly operation which occured since the number of items to pool wasn't equal to the pooled objects. Refresh the pool from the Projectile Pool component or the Pooling Window to prevent this in the future.");
            ClearPool();
            var numProjectiles = SpamPoolManager.GetNumProjectiles(_projectileAbility);
            if (EditorApplication.isPlaying)
                _projectilePool = SpamPoolManager.CreateProjectilePool(_projectileAbility, this, _decreaseNumProjectileAliveAction);
            else
                _projectilePool = new Pool<Projectile>(numProjectiles,
                    () => Projectile.CreateInstance(_projectileAbility, this.transform,
                        _decreaseNumProjectileAliveAction));
        }

        /// <summary>
        /// Clears the pool of instantiated projectiles.
        /// Only usable in editor.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public void ClearPool()
        {
            this.gameObject.DestroyAllChildren(true);
            _projectilePool?.Clear();
        }

        [Conditional("UNITY_EDITOR")]
        private void VerifyCorrectNumberOfProjectiles()
        {
            var numProjectiles = SpamPoolManager.GetNumProjectiles(this._projectileAbility);
            SpamLogger.EditorOnlyLog(_numInPool != 0 && numProjectiles != _numInPool,
                    $"The number of projectiles to spawn set on {this.gameObject.name} differs from the value in the pooling window. Number of instances is now controlled from the pooling window. To make this information go away, set Num In Pool to 0 on {this.gameObject.name}");
            
            if (0 < this.transform.childCount && this.transform.childCount != numProjectiles)
                RefillPool();
        }

        /// <summary>
        /// Gets the number of projectiles currently spawned in this pool.
        /// Only usable in editor
        /// </summary>
        public int GetProjectileCount()
        {
            return transform.childCount;
        }
#endif
    }
}