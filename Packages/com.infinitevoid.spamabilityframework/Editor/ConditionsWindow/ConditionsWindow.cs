using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core;
using InfiniteVoid.SpamFramework.Core.Components.Conditions;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Utils;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.ConditionsWindow
{
    public class ConditionsWindow : EditorWindow
    {
        private static IList<AbilityConditionSO> _conditions;
        private ListView _conditionsListView;
        
        [MenuItem("Tools/SPAM Framework/Conditions", false, 80)]
        public static void ShowWindow()
        {
            ConditionsWindow wnd = GetWindow<ConditionsWindow>("Conditions");
            wnd.minSize = new Vector2(400, 400);
            wnd.Show();
        }

        public static void ReloadConditions()
        {
            _conditions = GetAllConditions();
            ConditionsWindow wnd = GetWindow<ConditionsWindow>();
            wnd.titleContent = new GUIContent("Conditions");
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.itemsSource = _conditions.ToList();
#if UNITY_2021_3_OR_NEWER
            listView.Rebuild();
#else
            listView.Refresh();
#endif
            listView.SetSelection(0);
        }

        public static void SetSelectionAsSelected()
        {
            ConditionsWindow wnd = GetWindow<ConditionsWindow>();
            wnd.titleContent = new GUIContent("Conditions");
            var selectedIndex = _conditions.IndexOf(Selection.activeObject as AbilityConditionSO);
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.ClearSelection();
            listView.SetSelection(selectedIndex);
        }

        public void CreateGUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/ConditionsWindow/ConditionsWindow.uxml");

            var tree = visualTree.Instantiate();
            tree.StretchToParentSize();
            rootVisualElement.Add(tree);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/ConditionsWindow/ConditionsWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            var createNewButton = tree.Q<Button>("create-new-condition");
            createNewButton.clicked += SwitchToCreateNewConditionPage;


            _conditions = GetAllConditions();
            _conditionsListView = rootVisualElement.Q<ListView>(name: "conditions-list");
            _conditionsListView.styleSheets.Add(styleSheet);
            _conditionsListView.makeItem = () => new Label();
            _conditionsListView.bindItem = (e, i) =>
                (e as Label).text = string.IsNullOrWhiteSpace(_conditions[i].Name)
                    ? _conditions[i].name
                    : _conditions[i].Name;
            _conditionsListView.itemsSource = _conditions.ToList();
            _conditionsListView.selectionType = SelectionType.Single;
            _conditionsListView.onSelectionChange += OnConditionSelected;
        }

        public void SwitchToCreateNewConditionPage()
        {
            var conditionInfoContainer = rootVisualElement.Q<Box>("condition-info");
            conditionInfoContainer.style.display = DisplayStyle.None;
            var newConditionPage = rootVisualElement.Q<Box>("new-condition-page");
            newConditionPage.style.display = DisplayStyle.Flex;
            var newConditionContainer = rootVisualElement.Q<Box>("new-condition");
            newConditionContainer.Clear();
            newConditionContainer.Add(new CreateNewConditionPage(newConditionContainer));
        }

        private void OnConditionSelected(IEnumerable<object> selectedConditions)
        {
            var newCondtionPage = rootVisualElement.Q<Box>("new-condition-page");
            newCondtionPage.style.display = DisplayStyle.None;

            var conditionSo = (AbilityConditionSO)selectedConditions.FirstOrDefault();
            if (conditionSo == null)
                return;

            var conditionInfoContainer = rootVisualElement.Q<Box>("condition-info");
            conditionInfoContainer.style.display = DisplayStyle.Flex;

            var serializedCondition = new SerializedObject(conditionSo);
            var header = rootVisualElement.Q<Label>("condition-header");
            header.text
                = GetHeaderText(conditionSo);


            var goToAssetButton = rootVisualElement.Q<Button>("select-asset-button");
            goToAssetButton.clicked += () => { Selection.activeObject = conditionSo; };
            
            var renameAssetButton = rootVisualElement.Q<Button>("rename-asset-button");
            renameAssetButton.visible = conditionSo.name != conditionSo.Name && !conditionSo.Name.IsNullOrWhitespace();
            renameAssetButton.clicked += () =>
            {
                var path = AssetDatabase.GetAssetPath(conditionSo);
                AssetDatabase.RenameAsset(path, conditionSo.Name);
                header.text = GetHeaderText(conditionSo);
                renameAssetButton.visible = false;
            };


            var settingsWrapper = rootVisualElement.Q<Box>("condition-settings");
            settingsWrapper.Clear();
            var settingsBox = CreateBox("General settings", settingsWrapper);
            AddPropToElement("_name", serializedCondition, settingsBox, evt =>
            {
                header.text = GetHeaderText(conditionSo);
                renameAssetButton.visible = conditionSo.name != conditionSo.Name && !conditionSo.Name.IsNullOrWhitespace();
#if UNITY_2021_3_OR_NEWER
                _conditionsListView.Rebuild();
#else
                _conditionsListView.Refresh();
#endif
            });
            AddPropToElement("_description", serializedCondition, settingsBox);
            AddPropToElement("_addMultipleBehaviour", serializedCondition, settingsBox);
            
            settingsWrapper.Add(new ConditionVFXBox(serializedCondition, conditionSo));
            settingsWrapper.Add(new ConditionEffectsBox(serializedCondition, conditionSo));
        }


        private string GetHeaderText(AbilityConditionSO conditionSO)
        {
            if(string.IsNullOrWhiteSpace(conditionSO.Name))
                return conditionSO.name;

            return conditionSO.Name.ToLower() != conditionSO.name.ToLower()
                ? $"{conditionSO.Name} (asset: {conditionSO.name})"
                : conditionSO.Name;
        }

        private static IList<AbilityConditionSO> GetAllConditions()
        {
            return AssetDatabase
                .FindAssets("t:AbilityConditionSO", null)
                .Select(guid => AssetDatabase.LoadAssetAtPath<AbilityConditionSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(x => x.Name)
                .ToArray();
        }
    }
}