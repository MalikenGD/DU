using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Conditions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow.ConditionalEffectsBox
{
    public class ConditionalEffectsBoxItem : Box
    {
        public event Action RemoveConditionalEffectClick;
        public ConditionalEffectsBoxItem()
        {
            // var this = new Box();
            var button = new Button { name = "remove-condition" };
            this.Add(button);
            button.style.width = 20;
            button.AddToClassList("icon-minus");
            this.AddToClassList("c-preconditions-list-view__item");

            var objectField = new ObjectField();
            objectField.objectType = typeof(ConditionalEffectsSO);
            this.Add(objectField);

            var editEffectButton = new Button { name = "edit-conditional-effect" };
            editEffectButton.style.width = 20;
            editEffectButton.AddToClassList("icon-settings");
            this.Add(editEffectButton);
            
            this.Add(new Label() {name = "conditional-effect-desc"});
        }

        public void BindItem(SerializedProperty prop, AbilityBaseSO ability, SerializedObject serializedAbility,
            int itemIndex)
        {
            var conditionalEffectProp = prop.GetArrayElementAtIndex(itemIndex);
            var conditionalEffect = ability.ConditionalEffects[itemIndex];
            SetupConditionalEffectObjectField(itemIndex);
            SetupNameAndDescription(conditionalEffect);

            this.Q<Button>("remove-condition").clicked += () =>
            {
                ability.ConditionalEffects.RemoveAt(itemIndex);
                serializedAbility.Update();
                RemoveConditionalEffectClick?.Invoke();
            };
            this.Q<Button>("edit-conditional-effect").clicked += () =>
            {
                Selection.activeObject = conditionalEffect;
                ConditionalEffectsWindow.ConditionalEffectsWindow.SetSelectionAsSelected();
                EditorWindow.FocusWindowIfItsOpen<ConditionalEffectsWindow.ConditionalEffectsWindow>();
            };

            void SetupConditionalEffectObjectField(int i)
            {
                var effectObjectField = this.Q<ObjectField>();
                effectObjectField.value = conditionalEffectProp.objectReferenceValue;
                effectObjectField.RegisterValueChangedCallback(e =>
                {
                    ability.ConditionalEffects[i] = (ConditionalEffectsSO)e.newValue;
                    SetupNameAndDescription((ConditionalEffectsSO)e.newValue);
                    serializedAbility.Update();
                });
            }
        }

        private void SetupNameAndDescription(ConditionalEffectsSO conditionalEffectsSO)
        {
            var descLabel = this.Q<Label>("conditional-effect-desc");
            descLabel.text = descLabel.tooltip = conditionalEffectsSO ? conditionalEffectsSO.ToString() : string.Empty;
        }
    }
}