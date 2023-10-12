using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Conditions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.PreconditionsList
{
    public class PreconditionsListItem : Box
    {
        public event Action RemovePreconditionClick;
        public event Action PreconditionChanged;

        public PreconditionsListItem()
        {
            // var this = new Box();
            var button = new Button { name = "remove-condition" };
            this.Add(button);
            button.style.width = 20;
            button.AddToClassList("icon-minus");
            this.AddToClassList("c-preconditions-list-view__item");

            var constraintField = new EnumField();
            constraintField.Init(ConditionConstraintType.Has);
            this.Add(constraintField);

            var objectField = new ObjectField();
            objectField.objectType = typeof(AbilityConditionSO);
            this.Add(objectField);

            var editEffectButton = new Button { name = "edit-condition" };
            editEffectButton.style.width = 20;
            editEffectButton.AddToClassList("icon-settings");
            this.Add(editEffectButton);

            var onCasterToggle = new Toggle();
            onCasterToggle.style.marginLeft = 23;
            onCasterToggle.style.width = 30;
            onCasterToggle.tooltip = "Should the condition check be done on the caster instead of the target?";
            this.Add(onCasterToggle);
        }

        public void BindItem(SerializedProperty prop, AbilityBaseSO ability, SerializedObject serializedAbility,
            int itemIndex)
        {
            var preconditionProp = prop.GetArrayElementAtIndex(itemIndex);
            SetupConstraintObjectField(preconditionProp, itemIndex, "Condition", 
                (index, newValue) =>
                {
                    ability.PreConditions[index].Condition = newValue;
                    serializedAbility.Update();
                    PreconditionChanged?.Invoke();
                });
            SetupConstraintEnumField(preconditionProp,
                itemIndex,
                "Constraint",
                (index, newValue)
                    =>
                {
                    ability.PreConditions[index].Constraint = newValue;
                    serializedAbility.Update();
                    PreconditionChanged?.Invoke();
                });
            SetupConstraintToggleField(preconditionProp,
                itemIndex,
                "OnCaster",
                (index, newValue)
                    =>
                {
                    ability.PreConditions[index].OnCaster = newValue;
                    serializedAbility.Update();
                    PreconditionChanged?.Invoke();
                });
            
            this.Q<Button>("remove-condition").clicked += () =>
            {
                ability.PreConditions.RemoveAt(itemIndex);
                serializedAbility.Update();
                RemovePreconditionClick?.Invoke();
            };
            this.Q<Button>("edit-condition").clicked += () =>
            {
                Selection.activeObject = ability.PreConditions[itemIndex].Condition;
                ConditionsWindow.ConditionsWindow.SetSelectionAsSelected();
                EditorWindow.FocusWindowIfItsOpen<ConditionsWindow.ConditionsWindow>();
            };
        }

        public void BindItem(SerializedProperty prop, ConditionalEffectsSO conditionalEffectsSO,
            SerializedObject serializedCondition, int itemIndex)
        {
            var preconditionProp = prop.GetArrayElementAtIndex(itemIndex);
            var conditionConstraint = conditionalEffectsSO.Constraints[itemIndex];
            SetupConstraintObjectField(preconditionProp,
                itemIndex,
                "Condition",
                (index, newValue) =>
                {
                    conditionalEffectsSO.Constraints[index].Condition = newValue;
                    serializedCondition.Update();
                    PreconditionChanged?.Invoke();
                });
            SetupConstraintEnumField(preconditionProp,
                itemIndex,
                "Constraint",
                (index, newValue)
                    =>
                {
                    conditionalEffectsSO.Constraints[index].Constraint = newValue;
                    serializedCondition.Update();
                    PreconditionChanged?.Invoke();
                });
            SetupConstraintToggleField(preconditionProp,
                itemIndex,
                "OnCaster",
                (index, newValue)
                    =>
                {
                    conditionalEffectsSO.Constraints[index].OnCaster = newValue;
                    serializedCondition.Update();
                    PreconditionChanged?.Invoke();
                });


            this.Q<Button>("remove-condition").clicked += () =>
            {
                conditionalEffectsSO.Constraints.RemoveAt(itemIndex);
                serializedCondition.Update();
                RemovePreconditionClick?.Invoke();
            };
            this.Q<Button>("edit-condition").clicked += () =>
            {
                Selection.activeObject = conditionConstraint.Condition;
                ConditionsWindow.ConditionsWindow.SetSelectionAsSelected();
                EditorWindow.FocusWindowIfItsOpen<ConditionsWindow.ConditionsWindow>();
            };
        }

        private void SetupConstraintEnumField(
            SerializedProperty preconditionProp,
            int i,
            string constraintFieldName,
            Action<int, ConditionConstraintType> onValueChanged)
        {
            var constraintProperty = preconditionProp.FindPropertyRelative(constraintFieldName);
            var constraintField = this.Q<EnumField>();
            constraintField.value = (ConditionConstraintType)constraintProperty.enumValueIndex;
            constraintField.RegisterValueChangedCallback(e =>
                onValueChanged?.Invoke(i, (ConditionConstraintType)e.newValue));
        }
        
        private void SetupConstraintToggleField(
            SerializedProperty preconditionProp,
            int i,
            string constraintFieldName,
            Action<int, bool> onValueChanged)
        {
            var constraintProperty = preconditionProp.FindPropertyRelative(constraintFieldName);
            var constraintField = this.Q<Toggle>();
            constraintField.value = constraintProperty.boolValue;
            constraintField.RegisterValueChangedCallback(e =>
                onValueChanged?.Invoke(i, e.newValue));
        }


        private void SetupConstraintObjectField(
            SerializedProperty preconditionProp,
            int i,
            string conditionFieldName,
            Action<int, AbilityConditionSO> onValueChange)
        {
            var effectProp = preconditionProp.FindPropertyRelative(conditionFieldName);
            var effectObjectField = this.Q<ObjectField>();
            effectObjectField.value = effectProp.objectReferenceValue;
            effectObjectField.RegisterValueChangedCallback(
                e => onValueChange?.Invoke(i, (AbilityConditionSO)e.newValue));
            PreconditionChanged?.Invoke();
        }
    }
}