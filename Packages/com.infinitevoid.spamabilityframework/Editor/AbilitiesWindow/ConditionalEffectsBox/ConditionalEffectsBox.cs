using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow.ConditionalEffectsBox
{
    public class ConditionalEffectsBox : VisualElement
    {
        public ConditionalEffectsBox(AbilityBaseSO ability, SerializedObject serializedAbility)
        {
            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/ConditionalEffectsBox/ConditionalEffectsBox.uxml");
            visualTree.CloneTree(this);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/ConditionalEffectsBox/ConditionalEffectsBox.uss");
            this.styleSheets.Add(styleSheet);

            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            this.styleSheets.Add(styleSheet);

            var applyBeforeToggle = this.Q<Toggle>("apply-before-toggle");
            applyBeforeToggle.bindingPath = "_applyBeforeEffects";
            applyBeforeToggle.Bind(serializedAbility);
            
            var listview = this.Q<ListView>("conditional-effects-list");
            var prop = serializedAbility.FindProperty("_conditionalEffects");
            listview.itemsSource = ability.ConditionalEffects;
            listview.selectionType = SelectionType.None;
            listview.reorderable = true;
            listview.makeItem = () => new ConditionalEffectsBoxItem();
            listview.SetHeight(ability.ConditionalEffects.Count);
            listview.bindItem = (e, i) =>
            {
                serializedAbility.Update();
                var listItem = (ConditionalEffectsBoxItem)e;
                listItem.BindItem(prop, ability, serializedAbility, i);
                listItem.RemoveConditionalEffectClick += RefreshListView;
            };


            this.Q<Button>("create-condition-button").clicked += () =>
            {
                EditorWindow.FocusWindowIfItsOpen<ConditionalEffectsWindow.ConditionalEffectsWindow>();
                EditorWindow.GetWindow<ConditionalEffectsWindow.ConditionalEffectsWindow>()
                    .SwitchToCreateNewConditionalEffectPage();
            };

            this.Q<Button>("add-condition-button").clicked += () =>
            {
                ability.ConditionalEffects.Add(null);
                prop.isExpanded = true;
                serializedAbility.Update();
                RefreshListView();
            };


            void RefreshListView()
            {
                listview.SetHeight(ability.PreConditions.Count);
                listview.Reload();
            }
        }
    }
}