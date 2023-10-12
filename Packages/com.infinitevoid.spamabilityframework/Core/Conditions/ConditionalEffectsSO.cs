using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;
using UnityEngine.Serialization;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    public class ConditionalEffectsSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private List<ConditionConstraint> _constraints = new List<ConditionConstraint>();
        [SerializeField] private List<ConditionalEffectTarget> _effectTargets = new List<ConditionalEffectTarget>();

        public string Name => string.IsNullOrWhiteSpace(_name) ? name : _name;
        public List<ConditionConstraint> Constraints => _constraints;

        public List<ConditionalEffectTarget> EffectTargets => _effectTargets;

        public override string ToString()
        {
            var constraintsString = ConditionConstraint.Print(_constraints);
            var effectStringBuilder = new StringBuilder();
            var targetEffects = _effectTargets.Where(x => !x.OnCaster).ToArray();
            var casterEffects = _effectTargets.Where(x => x.OnCaster).ToArray();
            if (0 < targetEffects.Length)
            {
                effectStringBuilder.Append("[");
                effectStringBuilder.Append(string.Join(" and ",
                    targetEffects.Select(e => e?.Effect == null ? "NO EFFECT" : e.Effect.Name)));
                effectStringBuilder.Append("] to target");
            }

            if (0 < casterEffects.Length)
            {
                if (0 < effectStringBuilder.Length)
                    effectStringBuilder.Append(", and ");
                effectStringBuilder.Append("[");
                effectStringBuilder.Append(string.Join(" and ",
                    casterEffects.Select(e => e?.Effect == null ? "NO EFFECT" : e.Effect.Name)));
                effectStringBuilder.Append("] to caster");
            }

            if (effectStringBuilder.Length == 0)
                effectStringBuilder.Append("[NO EFFECTS SELECTED]");
            
            return $"If {constraintsString} apply {effectStringBuilder}";
        }

        public void ApplyEffects(IAbilityTarget target, Vector3 hitpos, IAbilityData ability, AbilityInvoker appliedBy)
        {
            for (var index = 0; index < _effectTargets.Count; index++)
            {
                _effectTargets[index].Effect.ApplyTo(
                    _effectTargets[index].OnCaster 
                        ? appliedBy 
                        : target, 
                    hitpos, ability, appliedBy);
            }
        }
    }
}