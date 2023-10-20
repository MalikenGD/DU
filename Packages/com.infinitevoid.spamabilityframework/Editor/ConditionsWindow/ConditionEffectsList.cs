using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.ConditionsWindow
{
    public class ConditionEffectsList : VisualElement
    {
        public ConditionEffectsList(AbilityConditionSO condition, SerializedObject serializedCondition)
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
            this.Q<Box>("effect-time-header").style.display = DisplayStyle.None;

            var listview = this.Q<ListView>("effect-list-view");
            var prop = serializedCondition.FindProperty("_secondaryEffects");
            listview.itemsSource = condition.SecondaryEffects;
            listview.selectionType = SelectionType.Single;
            listview.reorderable = true;
            listview.makeItem = MakeEffectListItem;
            listview.SetHeight(condition.SecondaryEffects.Count);
            listview.bindItem = (e, i) =>
            {
                serializedCondition.Update();
                var container = e as Box;
                var currentEffect = prop.GetArrayElementAtIndex(i);
                var effectAndTrigger = condition.SecondaryEffects[i];
                
                SetupEffectObjectField(currentEffect.FindPropertyRelative("Effect"), container, i);
                SetupEventsField(currentEffect.FindPropertyRelative("Events"), container, i);
                SetupNameAndDescription(container, effectAndTrigger);

                container.Q<Button>("remove-effect").clicked += () =>
                {
                    condition.SecondaryEffects.RemoveAt(i);
                    serializedCondition.Update();
                    listview.SetHeight(condition.SecondaryEffects.Count);
                    listview.Reload();
                };
                container.Q<Button>("edit-effect").clicked += () =>
                {
                    Selection.activeObject = effectAndTrigger.Effect;
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
                condition.SecondaryEffects.Add(new ConditionEffectAndEvents());
                prop.isExpanded = true;
                serializedCondition.Update();
                listview.SetHeight(condition.SecondaryEffects.Count);
                listview.Reload();
            };


            void SetupEffectObjectField(SerializedProperty effectProp, Box container, int i)
            {
                var effectObjectField = container.Q<ObjectField>();
                effectObjectField.value = effectProp.objectReferenceValue;
                effectObjectField.RegisterValueChangedCallback(e =>
                {
                    condition.SecondaryEffects[i].Effect = (AbilityEffectSO)e.newValue;
                    SetupNameAndDescription(container, condition.SecondaryEffects[i]);
                    serializedCondition.Update();
                });
            }

            void SetupEventsField(SerializedProperty triggersProp, Box container, int i)
            {
                var effectTriggerField = container.Q<EnumFlagsField>();
                effectTriggerField.value = (ConditionEffectEvent)triggersProp.intValue;
                effectTriggerField.RegisterValueChangedCallback(e =>
                {
                    condition.SecondaryEffects[i].Events = (ConditionEffectEvent)e.newValue;
                    SetupNameAndDescription(container, condition.SecondaryEffects[i]);
                    serializedCondition.Update();
                });
            }

            void SetupNameAndDescription(Box container, ConditionEffectAndEvents effectAndTriggers)
            {
                var infoLabel = container.Q<Label>("info-text");
                var effectName = effectAndTriggers == null || !effectAndTriggers.Effect || string.IsNullOrWhiteSpace(effectAndTriggers.Effect.Name)
                    ? string.Empty
                    : effectAndTriggers.Effect.ToString();
                string triggers = string.Empty;
                if (effectAndTriggers != null)
                {
                    triggers = (int)effectAndTriggers.Events == -1 
                        ? "(All) "
                        : $"({string.Join(",", effectAndTriggers.Events)}) ";
                }
                    
                infoLabel.text = infoLabel.tooltip = $"{triggers}{effectName}";
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

            var effectTriggerSelector = new EnumFlagsField(ConditionEffectEvent.Added)
                { name = "effect-triggers" };
            effectTriggerSelector.tooltip = "On which events should the effect be applied?";
            box.Add(effectTriggerSelector);

            var label = new Label();
            label.name = "info-text";
            box.Add(label);

            return box;
        }
    }
}