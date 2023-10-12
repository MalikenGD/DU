using InfiniteVoid.SpamFramework.Core.Conditions;
using UnityEditor;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.ConditionsWindow
{
    internal class ConditionEffectsBox : VisualElement
    {
        public ConditionEffectsBox(SerializedObject serializedCondition, AbilityConditionSO conditionSo)
        {
            var effectsBox = CreateBox("Secondary Effects", this);
            effectsBox.Add(new ConditionEffectsList(conditionSo, serializedCondition));
            
            effectsBox.Add(CreateHeader3("Effect settings", "u-margin-top"));
            AddPropToElement("_timeBetweenTicks", serializedCondition, effectsBox);
        }
    }
}