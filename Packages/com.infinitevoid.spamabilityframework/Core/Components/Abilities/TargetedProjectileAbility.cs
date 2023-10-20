using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components.Projectiles;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting
    /// of an <see cref="ProjectileAbilitySO"/> at a given <see cref="IAbilityTarget"/> or point 
    /// </summary>
    public class TargetedProjectileAbility : ProjectileAbility, ITargetedAbility
    {
        private IAbilityTarget _target;
        private Vector3 _position;
        public bool RequiresTarget => Ability.RequiresAbilityTarget;
        public bool CastOnSelf => false;
        private Projectile[] _projectiles;
        
        protected override void OnStart()
        {
            base.OnStart();
            _projectiles = new Projectile[base.ProjectileAbilitySo.SpawnedProjectiles.Count];
        }
 
        public void Cast(IAbilityTarget target)
        {
            _target = target;
            base.CastAbility(target.Transform.position);
        }
        
        public override void Cast(Vector3 position)
        {
            _position = position;
            base.CastAbility(position);
        }

        protected override void OnCast()
        {
            EnsureCorrectArraySize(ref _projectiles);
            base.PlayCastSFX();
            base.PlayCastVFX();
            base.GetProjectiles(_projectiles);
            for (int i = 0; i < _projectiles.Length; i++)
            {
                PreCastProjectileSetup?.Invoke(_projectiles[i]);
                if(_target != null)
                    _projectiles[i].MoveToTarget(_target, base.ProjectileAbilitySo, base.Invoker, base.ProjectileAbilitySo.SpawnedProjectiles[i]);
                else _projectiles[i].MoveTo(_position, base.ProjectileAbilitySo, base.Invoker, base.ProjectileAbilitySo.SpawnedProjectiles[i], ProjectileAbilitySo.DistanceCheckRange);
            }
            _target = null;
        }
    }
}