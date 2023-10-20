using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Conditions
{
    /// <summary>
    /// Allows the target to display certain VFX when a condition is applied.
    /// Requires an <seealso cref="AbilityConditionsTarget"/> and an <seealso cref="IAbilityTarget"/>
    /// </summary>
    [RequireComponent(typeof(AbilityConditionsTarget))]
    [RequireComponent(typeof(IAbilityTarget))]
    public class AbilityConditionsVFX : MonoBehaviour
    {
        [Tooltip("If set, this transform will be used as spawn point for VFX regardless of the settings of the condition.")]
        [SerializeField] private Transform _spawnPointOverride;
        private Transform _transform;
        private GameObject _vfxInstance;
        private AbilityConditionsTarget _abilityConditionsTarget;
        private static Dictionary<AbilityConditionSO, ConditionVFXPool> _conditionVfxPools;
        private ConditionVFX _currentConditionVfx;
        private IAbilityTarget _abilityTarget;
        private Vector3 _spawnPos;
        private AbilityConditionSO _currentCondition;

        // Reset pools if domain reload is disabled
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _conditionVfxPools = null;
        }

        private void Awake()
        {
            _transform = transform;
            TryGetComponent(out _abilityConditionsTarget);
            TryGetComponent(out _abilityTarget);
        }

        private void Start()
        {
            _abilityConditionsTarget.ConditionAdded += ShowConditionVFX;
            _abilityConditionsTarget.ConditionExpired += StopConditionVFX;
            _abilityConditionsTarget.ConditionRemoved += StopConditionVFX;
            _abilityConditionsTarget.ConditionTicked += OnConditionTicked;
            if (_conditionVfxPools is null)
            {
                var pools = FindObjectsOfType<ConditionVFXPool>();
                _conditionVfxPools = pools.ToDictionary(p => p.Condition, v => v);
            }
        }

        private void OnDestroy()
        {
            _abilityConditionsTarget.ConditionAdded -= ShowConditionVFX;
            _abilityConditionsTarget.ConditionExpired -= StopConditionVFX;
            _abilityConditionsTarget.ConditionRemoved -= StopConditionVFX;
            _abilityConditionsTarget.ConditionTicked -= OnConditionTicked;
        }

        private void StopConditionVFX(AbilityConditionSO condition)
        {
            if (_currentConditionVfx == null || !condition.IsSameAs(_currentCondition)) return;
            if (_abilityConditionsTarget.HasCondition(condition)) return;
            _currentConditionVfx.Stop();
            _currentConditionVfx = null;
            _currentCondition = null;
        }

        private void ShowConditionVFX(AbilityConditionSO condition, float _)
        {
            if (condition.VFX is null)
                return;
            if (condition.IsSameAs(_currentCondition)) return;
            if(_currentConditionVfx)
                _currentConditionVfx.Stop();
            SetSpawnPos(condition);
            foreach (var kv in _conditionVfxPools)
            {
                if (!kv.Key.IsSameAs(condition)) continue;
                _currentConditionVfx = kv.Value.GetNext(_spawnPos);
                _currentConditionVfx.FollowTarget(_transform);
                _currentCondition = condition;
                break;
            }
        }

        private void SetSpawnPos(AbilityConditionSO condition)
        {
            if (_spawnPointOverride)
            {
                _spawnPos = _spawnPointOverride.position;
            }
            else
            {
                _spawnPos = condition.SpawnAtTargetBase ? _abilityTarget.BasePosition : _transform.position;
                _spawnPos += condition.SpawnOffset;
            }
        }
        
        private void OnConditionTicked(AbilityConditionSO condition)
        {
            if (_currentCondition is null) return;
            if (!_currentCondition.IsSameAs(condition)) return;
            if ((_currentCondition.VFXEvents & ConditionEffectEvent.Ticked) != 0)
                _currentConditionVfx.Restart();

        }

    }
}