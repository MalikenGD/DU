using System;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Conditions;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Conditions
{
    public class ConditionVFXPool : MonoBehaviour
    {
        [SerializeField] private AbilityConditionSO _condition;
        private VfxPool _pool;

        public AbilityConditionSO Condition => _condition;

        void Start()
        {
            if (_condition == null || !_condition.VFX)
            {
                Debug.LogWarning("Condition VFX pool is created, but has no condition VFX to pool", this);
                return;
            }
            _pool = new VfxPool(_condition.NumPooled, () => Instantiate(_condition.VFX, this.transform));
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            this.gameObject.name = $"{_condition.Name} VFX Pool";
        }
#endif

        public void SetCondition(AbilityConditionSO condition)
        {
            _condition = condition;
        }

        public ConditionVFX GetNext(Vector3 position) => _pool.GetNext(position, true, Vector3.forward);

        private class VfxPool : Pool<ConditionVFX>
        {
            public VfxPool(int poolCount, Func<ConditionVFX> createPooledItem) : base(poolCount, createPooledItem)
            {
            }
        }
    }
}
