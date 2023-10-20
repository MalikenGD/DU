using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    [Serializable]
    public class ConditionConstraint
    {
        public ConditionConstraintType Constraint;
        public AbilityConditionSO Condition;
        public bool OnCaster;
        
        private const string SELECTED_CONDITION_TEXT = "SELECT CONDITION";

        public static string Print(IReadOnlyCollection<ConditionConstraint> constraints)
        {
            var targetConditionsStringBuilder = new StringBuilder();
            var targetHasConstraints = constraints.Where(x => x.Constraint == ConditionConstraintType.Has && !x.OnCaster).ToArray();
            var targetHasNotConstraints = constraints.Where(x => x.Constraint == ConditionConstraintType.HasNot && !x.OnCaster).ToArray();
            if (0 < targetHasConstraints.Length)
                targetConditionsStringBuilder.Append(
                    $"target has [{string.Join(" and ", targetHasConstraints.Select(x => x.Condition ? x.Condition.Name : SELECTED_CONDITION_TEXT))}]");
            if (0 < targetHasNotConstraints.Length)
            {
                if (0 < targetConditionsStringBuilder.Length)
                    targetConditionsStringBuilder.Append(" and not ");
                else targetConditionsStringBuilder.Append("target has not ");
                targetConditionsStringBuilder.Append($"[{string.Join(" or ", targetHasNotConstraints.Select(x => x.Condition ? x.Condition.Name : SELECTED_CONDITION_TEXT))}]");
            }
            var casterConditionsStringBuilder = new StringBuilder();
            var casterHasConstraints = constraints.Where(x => x.Constraint == ConditionConstraintType.Has && x.OnCaster).ToArray();
            var casterHasNotConstraints = constraints.Where(x => x.Constraint == ConditionConstraintType.HasNot && x.OnCaster).ToArray();
            if (0 < casterHasConstraints.Length)
            {
                if (0 < targetConditionsStringBuilder.Length)
                    casterConditionsStringBuilder.Append(", and ");
                casterConditionsStringBuilder.Append(
                    $"caster has [{string.Join(" and ", casterHasConstraints.Select(x => x.Condition ? x.Condition.Name : SELECTED_CONDITION_TEXT))}]");
            }
            if (0 < casterHasNotConstraints.Length)
            {
                if (0 < casterConditionsStringBuilder.Length)
                    casterConditionsStringBuilder.Append(" and not ");
                else
                {
                    if(0 < targetConditionsStringBuilder.Length)
                        casterConditionsStringBuilder.Append(" and caster has not ");
                    else
                        casterConditionsStringBuilder.Append("caster has not ");
                }
                casterConditionsStringBuilder.Append($"[{string.Join(" or ", casterHasNotConstraints.Select(x => x.Condition ? x.Condition.Name : SELECTED_CONDITION_TEXT))}]");
            }

            return $"{targetConditionsStringBuilder}{casterConditionsStringBuilder}";
        }
    }
}