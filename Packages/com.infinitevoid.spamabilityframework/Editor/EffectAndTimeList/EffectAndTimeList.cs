using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.EffectAndTimeList
{
    public class EffectAndTimeList : VisualElement
    {
        public EffectAndTimeList(AbilityBaseSO ability, SerializedObject serializedAbility)
        {
            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectAndTimeList/EffectAndTimeList.uxml");
            visualTree.CloneTree(this);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectAndTimeList/EffectAndTimeList.uss");
            this.styleSheets.Add(styleSheet);
            
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            this.styleSheets.Add(styleSheet);


            this.Q<Box>("conditional-effects-header").style.display = DisplayStyle.None;
            this.Q<Box>("condition-effects-header").style.display = DisplayStyle.None;
            
            var listview = this.Q<ListView>("effect-list-view");
            var prop = serializedAbility.FindProperty("_abilityEffects");
            listview.itemsSource = ability.AbilityEffects;
            listview.selectionType = SelectionType.Single;
            listview.reorderable = true;
            listview.makeItem = MakeEffectTimeListItem;
            listview.SetHeight(ability.AbilityEffects.Count);
            listview.bindItem = (e, i) =>
            {
                serializedAbility.Update();
                var container = e as Box;
                var effectAndTimeProp = prop.GetArrayElementAtIndex(i);
                var abilityEffect = ability.AbilityEffects[i].Effect;
                
                SetupEffectObjectField(effectAndTimeProp, container, i);
                SetupTimeFloatField(effectAndTimeProp, container, i);
                SetupNameAndDescription(container, abilityEffect);
                SetupOnCasterField(effectAndTimeProp, container, i);
                
                container.Q<Button>("remove-effect").clicked += () =>
                {
                    ability.AbilityEffects.RemoveAt(i);
                    serializedAbility.Update();
                    listview.SetHeight(ability.AbilityEffects.Count);
                    listview.Reload();
                };
                container.Q<Button>("edit-effect").clicked += () =>
                {
                    Selection.activeObject = abilityEffect;
                    EffectsWindow.EffectsWindow.SetSelectionAsSelected();
                    EditorWindow.FocusWindowIfItsOpen<SpamFramework.Editor.EffectsWindow.EffectsWindow>();
                };
            };

            this.Q<Button>("create-effect-button").clicked += () =>
            {
                EditorWindow.FocusWindowIfItsOpen<SpamFramework.Editor.EffectsWindow.EffectsWindow>();
                EditorWindow.GetWindow<SpamFramework.Editor.EffectsWindow.EffectsWindow>().SwitchToCreateNewEffectPage();
            };

            this.Q<Button>("add-effect-button").clicked += () =>
            {
                ability.AbilityEffects.Add(new EffectAndTime());
                prop.isExpanded = true;
                serializedAbility.Update();
                listview.SetHeight(ability.AbilityEffects.Count);
                listview.Reload();
            };
            

            void SetupEffectObjectField(SerializedProperty effectAndTimeProp, Box container, int i)
            {
                var effectProp = effectAndTimeProp.FindPropertyRelative("Effect");
                var effectObjectField = container.Q<ObjectField>();
                effectObjectField.value = effectProp.objectReferenceValue;
                effectObjectField.RegisterValueChangedCallback(e =>
                {
                    ability.AbilityEffects[i].Effect = (AbilityEffectSO)e.newValue;
                    SetupNameAndDescription(container, ability.AbilityEffects[i].Effect);
                    serializedAbility.Update();
                });
            }

            void SetupTimeFloatField(SerializedProperty effectAndTimeProp, Box container, int i)
            {
                var timeProp = effectAndTimeProp.FindPropertyRelative("EffectTime");
                var effectTimeFloatField = container.Q<FloatField>();
                effectTimeFloatField.value = timeProp.floatValue;
                effectTimeFloatField.RegisterValueChangedCallback(e =>
                {
                    ability.AbilityEffects[i].EffectTime = e.newValue;
                    serializedAbility.Update();
                });
            }
            
            void SetupOnCasterField(SerializedProperty effectAndTimeProp, Box container, int i)
            {
                var onCasterProp = effectAndTimeProp.FindPropertyRelative("ApplyOnCaster");
                var effectTimeFloatField = container.Q<Toggle>();
                effectTimeFloatField.value = onCasterProp.boolValue;
                effectTimeFloatField.RegisterValueChangedCallback(e =>
                {
                    ability.AbilityEffects[i].ApplyOnCaster = e.newValue;
                    serializedAbility.Update();
                });
            }


            void SetupNameAndDescription(Box container, AbilityEffectSO abilityEffect)
            {
                var infoLabel = container.Q<Label>("info-text");
                infoLabel.text = infoLabel.tooltip = abilityEffect ? abilityEffect.ToString() : String.Empty;
            }
        }

        private VisualElement MakeEffectTimeListItem()
        {
            var box = new Box();
            var button = new Button {name ="remove-effect"};
            box.Add(button);
            button.style.width = 20;
            button.AddToClassList("icon-minus");
            box.AddToClassList("c-effects-list-view__item");

            var objectField = new ObjectField();
            objectField.objectType = typeof(AbilityEffectSO);
            box.Add(objectField);

            var editEffectButton = new Button {name="edit-effect"};
            editEffectButton.style.width = 20;
            editEffectButton.AddToClassList("icon-settings");
            box.Add(editEffectButton);
            
            var timeField = new FloatField();
            // timeField.label = "Time";
            timeField.tooltip =
                "The time it takes for the effect to be applied. Adds a delay between this and the next effect in the list.";
            box.Add(timeField);

            var onCasterField = new Toggle();
            onCasterField.style.marginLeft = 25;
            onCasterField.style.width = 35;
            onCasterField.tooltip = "Should the effect be applied to the caster instead of the target?";
            box.Add(onCasterField);

            var label = new Label();
            label.name = "info-text";
            box.Add(label);
            
            return box;
        }
    }
}