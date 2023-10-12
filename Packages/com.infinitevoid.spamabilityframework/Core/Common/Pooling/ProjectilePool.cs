using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;
using UnityEngine.Serialization;

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    /// <summary>
    /// A specialized pool for handling projectiles 
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private ProjectileAbilitySO _projectileInPool;
        [FormerlySerializedAs("_noInPool")] [SerializeField] private int _numInPool = 1;
        [HelpBox("When abilities that reference this pool are destroyed they will call Destroy on this pool. Check this if you want to manage its lifetime manually.")]
        [SerializeField] private bool _keepAlive;
        private Pool<Projectile> _projectilePool;
        private Pool<Transform> _onHitVfxPool;
        private GameObject _onHitVFXToSpawn;
        private Func<Transform> _spawnVfxAction;
        private Transform _transform;
        private int _numProjectilesAlive = 0;
        private Action _decreaseNumProjectileAliveAction;

        public ProjectileAbilitySO ProjectileAbilitySo => _projectileInPool;

        private void Awake()
        {
            _transform = this.transform;
            _decreaseNumProjectileAliveAction = () => this._numProjectilesAlive--;
            this.enabled = false;
            if (_projectileInPool != null)
                InitPool();
            // _spawnVfxAction = InstantiateOnHitVFX;
        }

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
            
            SpamLogger.EditorOnlyErrorLog(_projectileInPool.Prefab == null, $"No projectile assigned to {_projectileInPool.Name}");
            _projectilePool = new Pool<Projectile>(_numInPool, () =>
            {
                var projectileInstance = Instantiate(_projectileInPool.Prefab, this._transform);
                projectileInstance.gameObject.SetActive(false);
                projectileInstance.Init(ProjectileAbilitySo, _transform, _decreaseNumProjectileAliveAction);
                return projectileInstance;
            });
        }

        /// <summary>
        /// Initializes the pool, and optionally creates all pooled items.
        /// </summary>
        /// <param name="projectileAbilitySo"></param>
        /// <param name="createItemsImmediately"></param>
        public void Init(ProjectileAbilitySO projectileAbilitySo, bool createItemsImmediately = false)
        {
            _projectileInPool = projectileAbilitySo;
            if(createItemsImmediately)
                InitPool();
        }


        public void SetProjectile(ProjectileAbilitySO projectileAbility)
        {
            if (!this._transform)
                this._transform = transform;

            _projectileInPool = projectileAbility;
            foreach (var t in _projectilePool.Items)
            {
                t.Init(projectileAbility, _transform, _decreaseNumProjectileAliveAction);
            }
        }

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
    }
}