using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Conditions;

namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    public interface IAbilityConditionsTarget
    {
        void AddCondition(AbilityConditionSO condition, float lifetime, AbilityInvoker caster);
        void RemoveCondition(AbilityConditionSO condition);
    }
}