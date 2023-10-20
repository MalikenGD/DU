using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for a projectile ability
    /// </summary>
    [CreateAssetMenu(menuName = "SPAM Framework/Abilities/Projectile Ability", fileName = "projectileAbility.asset")]
    public class ProjectileAbilitySO : AbilityBaseSO
    {
        [Header("Projectile settings")] 
        [Tooltip("Set this to 0 if the projectile should never die")] 
        [SerializeField] private float _timeToLive;
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private AudioClip _inFlightSound;
        [SerializeField] private OnHitAction _onHitAction = OnHitAction.None;
        [SerializeField] private float _distanceCheckRange = .1f;
        [SerializeField] private List<ProjectileSettings> _spawnedProjectiles = new List<ProjectileSettings>();

        public float TimeToLive => _timeToLive;
        public Projectile Prefab => _projectilePrefab;
        public bool DeactivateOnHit => _onHitAction == OnHitAction.Deactivate;
        public float DistanceCheckRange => _distanceCheckRange;
        public AudioClip InFlightSound => _inFlightSound;
        public bool StopOnHit => _onHitAction == OnHitAction.Disable;

        public List<ProjectileSettings> SpawnedProjectiles => _spawnedProjectiles;

        public AbilityBase AddTo(GameObject gameObject, Transform projectileSpawn, Transform warmupVfxSpawn, bool directional)
        {
            ProjectileAbility component;
            if (directional)
                component = gameObject.AddComponent<DirectionalProjectileAbility>();
            else component = gameObject.AddComponent<TargetedProjectileAbility>();

            var pool = SpamPoolManager.GetOrCreateProjectilePoolInScene(this);
            component.SetAbility(this);
            component.SetPool(pool);
            component.SetProjectileSpawnPoint(projectileSpawn);
            component.SetWarmupVFXSpawn(warmupVfxSpawn);
            component.GetInvokerFromHierarchy();
            return component;
        }

        public bool IsSameAs(ProjectileAbilitySO other) => this.GetInstanceID() == other.GetInstanceID();


        public override bool TargetIsInSight(AbilityInvoker _, IAbilityTarget potentialTarget, Vector3 hitPosition,
            Vector3 lookDirection)
        {
            if(!this.LineOfSightCheck)
                return true;
                
            return !Physics.Linecast(hitPosition, potentialTarget.Transform.position, this.LosBlockingLayers);
        }


        private enum OnHitAction
        {
            None,
            Disable,
            Deactivate
        }
    }
}