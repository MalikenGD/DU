using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for a raycast ability
    /// </summary>
    [CreateAssetMenu(menuName = "SPAM Framework/Abilities/Raycast Ability", fileName = "raycastAbility.asset")]
    public class RaycastAbilitySO : AbilityBaseSO
    {
        [SerializeField] private float _raycastLength;
        [SerializeField] private LayerMask _raycastLayers;
        public AbilityBase AddTo(GameObject gameObject, Transform warmupVfxSpawn)
        {
            return null;
        }

        public override bool TargetIsInSight(AbilityInvoker invoker, IAbilityTarget potentialTarget, Vector3 hitPosition,
            Vector3 lookDirection)
        {
            return true;
        }

        public float RaycastLength => _raycastLength <= 0 ? Mathf.Infinity : _raycastLength;

        public LayerMask RaycastLayers => _raycastLayers;
    }
}