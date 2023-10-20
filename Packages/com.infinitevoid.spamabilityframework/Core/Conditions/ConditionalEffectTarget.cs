using InfiniteVoid.SpamFramework.Core.Effects;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    /// <summary>
    /// A combination of an effect and a target.
    /// Used in <see cref="ConditionalEffectsSO"/> so effects can be applied to either caster or target
    /// </summary>
    [System.Serializable]
    public class ConditionalEffectTarget
    {
        public AbilityEffectSO Effect;
        public bool OnCaster;
    }
}