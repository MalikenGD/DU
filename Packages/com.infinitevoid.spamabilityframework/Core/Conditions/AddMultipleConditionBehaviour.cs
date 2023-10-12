namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    /// <summary>
    /// The behaviour to use when adding multiple conditions of the same type to a target
    /// </summary>
    public enum AddMultipleConditionBehaviour
    {
        DoNothing = 0,
        Extend = 1,
        Overwrite = 2,
        Stack = 3
    }
}