using System;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.ConditionalEffectsWindow
{
    public class EffectsList : VisualElement
    {
        public event Action EffectRemoved;
        public event Action EffectChanged;
        public EffectsList(ConditionalEffectsSO conditionalEffectsSO, SerializedObject serializedCondition)
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

            this.Q<Box>("effect-time-header").style.display = DisplayStyle.None;
            this.Q<Box>("condition-effects-header").style.display = DisplayStyle.None;
            
            var listview = this.Q<ListView>("effect-list-view");
            var prop = serializedCondition.FindProperty("_effectTargets");
            listview.itemsSource = conditionalEffectsSO.EffectTargets;
            listview.selectionType = SelectionType.Single;
            listview.reorderable = true;
            listview.makeItem = MakeEffectListItem;
            listview.SetHeight(conditionalEffectsSO.EffectTargets.Count);
            listview.bindItem = (e, i) =>
            {
                serializedCondition.Update();
                var container = e as Box;
                var currentEffect = prop.GetArrayElementAtIndex(i);
                var effect = conditionalEffectsSO.EffectTargets[i].Effect;

                SetupEffectObjectField(currentEffect.FindPropertyRelative("Effect"), container, i);
                SetupOnCasterBool(currentEffect.FindPropertyRelative("OnCaster"), container, i);
                SetupNameAndDescription(container, effect);

                container.Q<Button>("remove-effect").clicked += () =>
                {
                    conditionalEffectsSO.EffectTargets.RemoveAt(i);
                    serializedCondition.Update();
                    listview.SetHeight(conditionalEffectsSO.EffectTargets.Count);
                    EffectRemoved?.Invoke();
                    listview.Reload();
                };
                container.Q<Button>("edit-effect").clicked += () =>
                {
                    Selection.activeObject = effect;
                    EffectsWindow.EffectsWindow.SetSelectionAsSelected();
                    EditorWindow.FocusWindowIfItsOpen<SpamFramework.Editor.EffectsWindow.EffectsWindow>();
                };
            };

            this.Q<Button>("create-effect-button").clicked += () =>
            {
                EditorWindow.FocusWindowIfItsOpen<SpamFramework.Editor.EffectsWindow.EffectsWindow>();
                EditorWindow.GetWindow<SpamFramework.Editor.EffectsWindow.EffectsWindow>()
                    .SwitchToCreateNewEffectPage();
            };

            this.Q<Button>("add-effect-button").clicked += () =>
            {
                conditionalEffectsSO.EffectTargets.Add(null);
                prop.isExpanded = true;
                serializedCondition.Update();
                listview.SetHeight(conditionalEffectsSO.EffectTargets.Count);
                listview.Reload();
            };

            void SetupEffectObjectField(SerializedProperty effectProp, Box container, int i)
            {
                var effectObjectField = container.Q<ObjectField>();
                effectObjectField.value = effectProp.objectReferenceValue;
                effectObjectField.RegisterValueChangedCallback(e =>
                {
                    conditionalEffectsSO.EffectTargets[i].Effect = (AbilityEffectSO)e.newValue;
                    SetupNameAndDescription(container, conditionalEffectsSO.EffectTargets[i].Effect);
                    serializedCondition.Update();
                    EffectChanged?.Invoke();
                });
            }
            
            void SetupOnCasterBool(SerializedProperty effectProp, Box container, int i)
            {
                var onCasterToggle = container.Q<Toggle>();
                onCasterToggle.value = effectProp.boolValue;
                onCasterToggle.RegisterValueChangedCallback(e =>
                {
                    conditionalEffectsSO.EffectTargets[i].OnCaster = e.newValue;
                    SetupNameAndDescription(container, conditionalEffectsSO.EffectTargets[i].Effect);
                    serializedCondition.Update();
                    EffectChanged?.Invoke();
                });
            }


            void SetupNameAndDescription(Box container, AbilityEffectSO effect)
            {
                var infoLabel = container.Q<Label>("info-text");

                infoLabel.text = infoLabel.tooltip = effect ? effect.ToString() : string.Empty;
            }
        }

        private VisualElement MakeEffectListItem()
        {
            var box = new Box();
            var button = new Button { name = "remove-effect" };
            box.Add(button);
            button.style.width = 20;
            button.AddToClassList("icon-minus");
            box.AddToClassList("c-effects-list-view__item");

            var objectField = new ObjectField();
            objectField.objectType = typeof(AbilityEffectSO);
            box.Add(objectField);

            var editEffectButton = new Button { name = "edit-effect" };
            editEffectButton.style.width = 20;
            editEffectButton.AddToClassList("icon-settings");
            box.Add(editEffectButton);
            
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