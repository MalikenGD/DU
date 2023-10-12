using System;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    /// <summary>
    /// The event that trigger application of secondary effects.
    /// </summary>
    [Flags]
    public enum ConditionEffectEvent
    {
        Added = 1,
        Ticked = 2,
        Extended = 4,
        Expired = 8,
        Removed = 16,
    }
}