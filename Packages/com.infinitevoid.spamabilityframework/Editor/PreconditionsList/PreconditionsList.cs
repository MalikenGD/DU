using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.PreconditionsList
{
    public class PreconditionsList : VisualElement
    {
        public event Action PreconditionChanged;
        public event Action PreconditionRemoved;
        public PreconditionsList(ConditionalEffectsSO conditionalEffectsSO, SerializedObject serializedCondition)
        {
            SetupVisualTree();
            
            var listview = this.Q<ListView>("preconditions-list-view");
            
            var prop = serializedCondition.FindProperty("_constraints");
            listview.itemsSource = conditionalEffectsSO.Constraints;
            listview.selectionType = SelectionType.None;
            listview.reorderable = true;
            listview.makeItem = () => new PreconditionsListItem();
            listview.SetHeight(conditionalEffectsSO.Constraints.Count);
            listview.bindItem = (e, i) =>
            {
                serializedCondition.Update();
                var listItem = (PreconditionsListItem)e;
                listItem.BindItem(prop, conditionalEffectsSO, serializedCondition, i);
                listItem.RemovePreconditionClick += () =>
                {
                    RefreshListView(listview, conditionalEffectsSO.Constraints.Count);
                    PreconditionRemoved?.Invoke();
                };
                listItem.PreconditionChanged += () => PreconditionChanged?.Invoke();
            };


            this.Q<Button>("create-condition-button").clicked += () =>
            {
                EditorWindow.FocusWindowIfItsOpen<ConditionsWindow.ConditionsWindow>();
                EditorWindow.GetWindow<ConditionsWindow.ConditionsWindow>()
                    .SwitchToCreateNewConditionPage();
            };

            this.Q<Button>("add-condition-button").clicked += () =>
            {
                conditionalEffectsSO.Constraints.Add(new ConditionConstraint());
                prop.isExpanded = true;
                serializedCondition.Update();
                PreconditionChanged?.Invoke();
                RefreshListView(listview, conditionalEffectsSO.Constraints.Count);
            };

            
        }
        public PreconditionsList(AbilityBaseSO ability, SerializedObject serializedAbility)
        {
            SetupVisualTree();


            var listview = this.Q<ListView>("preconditions-list-view");
            var prop = serializedAbility.FindProperty("_conditionConstraints");
            listview.itemsSource = ability.PreConditions;
            listview.selectionType = SelectionType.None;
            listview.reorderable = true;
            listview.makeItem = () => new PreconditionsListItem();
            listview.SetHeight(ability.PreConditions.Count);
            listview.bindItem = (e, i) =>
            {
                serializedAbility.Update();
                var listItem = (PreconditionsListItem)e;
                listItem.BindItem(prop, ability, serializedAbility, i);
                listItem.RemovePreconditionClick += () =>
                {
                    RefreshListView(listview, ability.PreConditions.Count);
                    PreconditionRemoved?.Invoke();
                };
                listItem.PreconditionChanged += () => PreconditionChanged?.Invoke();
            };


            this.Q<Button>("create-condition-button").clicked += () =>
            {
                EditorWindow.FocusWindowIfItsOpen<ConditionsWindow.ConditionsWindow>();
                EditorWindow.GetWindow<ConditionsWindow.ConditionsWindow>()
                    .SwitchToCreateNewConditionPage();
            };

            this.Q<Button>("add-condition-button").clicked += () =>
            {
                ability.PreConditions.Add(new ConditionConstraint());
                prop.isExpanded = true;
                serializedAbility.Update();
                PreconditionChanged?.Invoke();
                RefreshListView(listview, ability.PreConditions.Count);
            };


        }

        private void SetupVisualTree()
        {
            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/PreconditionsList/PreconditionsList.uxml");
            visualTree.CloneTree(this);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/PreconditionsList/PreconditionsList.uss");
            this.styleSheets.Add(styleSheet);

            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            this.styleSheets.Add(styleSheet);
        }
        
        void RefreshListView(ListView listview, int numItems)
        {
            listview.SetHeight(numItems);
            listview.Reload();
        }
    }
}