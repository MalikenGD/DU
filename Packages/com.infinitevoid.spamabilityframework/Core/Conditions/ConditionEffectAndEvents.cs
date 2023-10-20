using InfiniteVoid.SpamFramework.Core.Effects;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    /// <summary>
    /// An object representing a condition-effect (<see cref="AbilityEffectSO"/>) and a collection of triggers (when it should be applied)
    /// </summary>
    [System.Serializable]
    public class ConditionEffectAndEvents
    {
        public AbilityEffectSO Effect;
        public ConditionEffectEvent Events = ConditionEffectEvent.Added;
    }
}