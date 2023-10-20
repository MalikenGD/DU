using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Meshes;
using InfiniteVoid.SpamFramework.Core.Extensions;
using UnityEngine;
using Plane = InfiniteVoid.SpamFramework.Core.Common.Meshes.Plane;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of an <see cref="DirectionalAbilitySO"/>
    /// </summary>
    public class DirectionalAbility : AbilityBase
    {
        [SerializeField] private DirectionalAbilitySO _directionalAbility;
        protected override AbilityBaseSO Ability => _directionalAbility;

        /// <summary>
        /// Only applicable if <see cref="ConeAOE"/> is true.
        /// </summary>
        public float Angle => _directionalAbility.Angle;
        public float Distance => _directionalAbility.Distance;
        public bool ConeAOE => _directionalAbility.ConeAOE;

        public void SetAbility(DirectionalAbilitySO ability)
        {
            this._directionalAbility = ability;
        }
        
        protected override void OnCast()
        {
            var impactPoint = new ImpactPoint(Invoker.Position + Invoker.Forward * (Distance / 2), Vector3.up);
            PlayCastSFX();
            PlayCastVFX();
            PlayImpactVFX(impactPoint);
            AbilityHit(impactPoint);
        }
        
        
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _visualizeAoe;
        [SerializeField] private Color _gizmoColor = new Color(144, 254, 254);
        [SerializeField] private Quaternion _gizmoRotation = Quaternion.identity;
        [SerializeField] private bool _gizmoFollowCharacterForward;
        
        
        private Mesh _visualizedMesh;

        private void OnDrawGizmos()
        {
            if(!_visualizeAoe) return;
            if (_directionalAbility.ConeAOE)
            {
                _visualizedMesh = new WedgePlane(Distance, Angle, false).Mesh;
                Gizmos.color = _gizmoColor;
                Gizmos.DrawMesh(_visualizedMesh, transform.position.Add(y: .1f), _gizmoFollowCharacterForward ? transform.root.rotation : _gizmoRotation);
            }
            else
            {
                Gizmos.color = _gizmoColor;
                _visualizedMesh = new Plane(new Vector2(_directionalAbility.Width, Distance)).Mesh;
                Gizmos.DrawMesh(_visualizedMesh, transform.position.Add(y: .1f), _gizmoFollowCharacterForward ? transform.root.rotation : _gizmoRotation);
            }
        }
#endif
    }
}