using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Conditions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    [CreateAssetMenu(menuName = "SPAM Framework/Ability condition", fileName = "abilityCondition.asset")]
    public class AbilityConditionSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [Tooltip("What should happen when a condition is added to a target that already has the condition?")]
        [SerializeField] private AddMultipleConditionBehaviour _addMultipleBehaviour;
        
        [Header("VFX")]
        [SerializeField] private ConditionVFX _vfx;
        [SerializeField] private bool _spawnAtTargetBase;
        [SerializeField] private Vector3 _spawnOffset;
        [SerializeField] private int _numPooled;
        [SerializeField] private ConditionEffectEvent _playOnEvents;

        [Header("Secondary effects")] 
        [SerializeField] private List<ConditionEffectAndEvents> _secondaryEffects = new List<ConditionEffectAndEvents>();

        [Tooltip("If set to 0, effects will not tick (regardless of triggers) and only be applied on add/expired/removed.")]
        [SerializeField] private float _timeBetweenTicks;

        public string Name => _name;
        public bool SpawnAtTargetBase => _spawnAtTargetBase;
        public Vector3 SpawnOffset => _spawnOffset;
        public string Description => _description;
        public ConditionVFX VFX => _vfx;
        public int NumPooled => _numPooled;
        public ConditionEffectEvent VFXEvents => _playOnEvents;
        public List<ConditionEffectAndEvents> SecondaryEffects => _secondaryEffects;
        public bool HasTickedEffects => HasSecondaryEffects && 0 < _timeBetweenTicks;
        public bool HasSecondaryEffects => 0 < _secondaryEffects.Count;
        public float TimeBetweenTicks => _timeBetweenTicks;
        public bool Stackable => _addMultipleBehaviour == AddMultipleConditionBehaviour.Stack;
        public bool ExtendOnAddMultiple => _addMultipleBehaviour == AddMultipleConditionBehaviour.Extend;
        public bool OverwriteOnAddMultiple => _addMultipleBehaviour == AddMultipleConditionBehaviour.Overwrite;
        public AddMultipleConditionBehaviour AddMultipleBehaviour => _addMultipleBehaviour;

        /// <summary>
        /// The template condition (i.e. the one created in the Editor) that was used to create this instance.
        /// This is used to compare two conditions two each other
        /// </summary>
        [HideInInspector, SerializeField] private AbilityConditionSO _assetTemplate;

        public AbilityConditionSO CreateInstance()
        {
            var instance = Instantiate(this);
            instance._assetTemplate = this;

            return instance;
        }


        public bool IsSameAs(AbilityConditionSO otherCondition)
        {
            if (otherCondition is null) return false;

            // test in-case either item being compared was an asset
            if (this._assetTemplate is null)
            {
                return ReferenceEquals(this, otherCondition) || ReferenceEquals(this, otherCondition._assetTemplate);
            }

            if (otherCondition._assetTemplate is null)
            {
                return ReferenceEquals(this, otherCondition) || ReferenceEquals(this._assetTemplate, otherCondition);
            }

            return ReferenceEquals(this._assetTemplate, otherCondition._assetTemplate);
        }

        public void ApplySecondaryEffects(IAbilityTarget target, Vector3 position, AbilityInvoker appliedBy,
            ConditionEffectEvent conditionEffectEvent)
        {
            foreach (var secondaryEffect in _secondaryEffects)
            {
                SpamLogger.EditorOnlyErrorLog(!secondaryEffect.Effect, $"No effect assigned for secondary effects of condition {this.Name}");
                if ((secondaryEffect.Events & conditionEffectEvent) != 0)
                {
                    secondaryEffect.Effect.ApplyTo(target, position, null, appliedBy);
                }
            }
        }

        /// <summary>
        /// Used only by tests to allow overwrite of behaviour when creating instances through code.
        /// Refrain from using this outside of tests.
        /// </summary>
        /// <param name="behaviour"></param>
        public void SetAddBehaviour(AddMultipleConditionBehaviour behaviour) => _addMultipleBehaviour = behaviour;
    }
}