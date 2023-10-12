using System;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    /// <summary>
    /// An object representing an Ability-effect and it's timing for applying the effect to the target.
    /// </summary>
    [Serializable]
    public class EffectAndTime
    {
        public AbilityEffectSO Effect;
        public float EffectTime;
        public bool ApplyOnCaster;
    }
}