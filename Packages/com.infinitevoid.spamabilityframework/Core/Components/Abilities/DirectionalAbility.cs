using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Meshes;
using InfiniteVoid.SpamFramework.Core.Extensions;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of an <see cref="DirectionalAbilitySO"/>
    /// </summary>
    public class DirectionalAbility : AbilityBase
    {
        [SerializeField] private DirectionalAbilitySO _directionalAbility;
        protected override AbilityBaseSO Ability => _directionalAbility;

        public float Angle => _directionalAbility.Angle;
        public float Distance => _directionalAbility.Distance;

        public void SetAbility(DirectionalAbilitySO ability)
        {
            this._directionalAbility = ability;
        }
        
        
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _visualizeAoe;
        [SerializeField] private Color _gizmoColor = new Color(144, 254, 254);

        private WedgePlane _visualizedMesh;

        private void OnDrawGizmos()
        {
            if(!_visualizeAoe) return;
            _visualizedMesh = new WedgePlane(Distance, Angle, false);
            Gizmos.color = _gizmoColor;
            Gizmos.DrawMesh(_visualizedMesh.Mesh, transform.position.Add(y: .1f), transform.root.rotation);
        }
#endif
    }
}