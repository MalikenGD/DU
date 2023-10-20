using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Utils;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using InfiniteVoid.SpamFramework.Editor.ConditionsWindow;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.ConditionalEffectsWindow
{
    public class ConditionalEffectsWindow : EditorWindow
    {
        private static IList<ConditionalEffectsSO> _conditionalEffects;
        private ListView _conditionsListView;

        [MenuItem("Tools/SPAM Framework/Conditional effects", false, 85)]
        public static void ShowWindow()
        {
            ConditionalEffectsWindow wnd = GetWindow<ConditionalEffectsWindow>("Conditional effects");
            wnd.minSize = new Vector2(400, 400);
            wnd.titleContent = EditorUtils.GetSPAMWindowTitle("Conditional effects");
            wnd.Show();
        }

        public static void ReloadConditionalEffects()
        {
            _conditionalEffects = GetAllConditionalEffects();
            ConditionalEffectsWindow wnd = GetWindow<ConditionalEffectsWindow>();
            wnd.titleContent = new GUIContent("Conditional effects");
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.itemsSource = _conditionalEffects.ToList();
            listView.Reload();
            listView.SetSelection(0);
        }

        public static void SetSelectionAsSelected()
        {
            ConditionalEffectsWindow wnd = GetWindow<ConditionalEffectsWindow>();
            wnd.titleContent = new GUIContent("Conditional effects");
            var selectedIndex = _conditionalEffects.IndexOf(Selection.activeObject as ConditionalEffectsSO);
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.ClearSelection();
            listView.SetSelection(selectedIndex);
        }

        public void CreateGUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/ConditionalEffectsWindow/ConditionalEffectsWindow.uxml");

            var tree = visualTree.Instantiate();
            tree.StretchToParentSize();
            rootVisualElement.Add(tree);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/ConditionalEffectsWindow/ConditionalEffectsWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            var createNewButton = tree.Q<Button>("create-new-condition");
            createNewButton.clicked += SwitchToCreateNewConditionalEffectPage;


            _conditionalEffects = GetAllConditionalEffects();
            _conditionsListView = rootVisualElement.Q<ListView>("conditional-effects-list");
            _conditionsListView.styleSheets.Add(styleSheet);
            _conditionsListView.makeItem = () => new Label();
            _conditionsListView.bindItem = (e, i) =>
                (e as Label).text = string.IsNullOrWhiteSpace(_conditionalEffects[i].Name)
                    ? _conditionalEffects[i].name
                    : _conditionalEffects[i].Name;
            _conditionsListView.itemsSource = _conditionalEffects.ToList();
            _conditionsListView.selectionType = SelectionType.Single;
            _conditionsListView.onSelectionChange += OnConditionalEffectSelected;
        }

        public void SwitchToCreateNewConditionalEffectPage()
        {
            var conditionInfoContainer = rootVisualElement.Q<Box>("condition-info");
            conditionInfoContainer.style.display = DisplayStyle.None;
            var newConditionPage = rootVisualElement.Q<Box>("new-condition-page");
            newConditionPage.style.display = DisplayStyle.Flex;

            newConditionPage.Add(new CreateNewConditionalEffectsPage(newConditionPage));
        }

        private void OnConditionalEffectSelected(IEnumerable<object> selectedConditionalEffects)
        {
            var newCondtionPage = rootVisualElement.Q<Box>("new-condition-page");
            newCondtionPage.style.display = DisplayStyle.None;

            var conditionalEffectsSO = (ConditionalEffectsSO)selectedConditionalEffects.FirstOrDefault();
            if (conditionalEffectsSO == null)
                return;

            var conditionInfoContainer = rootVisualElement.Q<Box>("condition-info");
            conditionInfoContainer.style.display = DisplayStyle.Flex;

            var serializedConditionalEffect = new SerializedObject(conditionalEffectsSO);

            var headerLabel = rootVisualElement.Q<Label>("conditional-effect-header");
            headerLabel.text = GetHeaderText(conditionalEffectsSO);
            SetDescription(conditionalEffectsSO);

            var goToAssetButton = rootVisualElement.Q<Button>("select-asset-button");
            goToAssetButton.clicked += () => { Selection.activeObject = conditionalEffectsSO; };

            var renameAssetButton = rootVisualElement.Q<Button>("rename-asset-button");
            renameAssetButton.visible = conditionalEffectsSO.name != conditionalEffectsSO.Name &&
                                        !conditionalEffectsSO.Name.IsNullOrWhitespace();
            renameAssetButton.clicked += () =>
            {
                var path = AssetDatabase.GetAssetPath(conditionalEffectsSO);
                AssetDatabase.RenameAsset(path, conditionalEffectsSO.Name);
                headerLabel.text = GetHeaderText(conditionalEffectsSO);
                renameAssetButton.visible = false;
            };


            var settingsWrapper = rootVisualElement.Q<Box>("conditional-effect-settings");
            settingsWrapper.Clear();

            var generalSettingsBox = CreateBox("General settings", settingsWrapper,
                helpUrl: "https://spam.infinitevoid.games/conditional-effects.html#general-settings");
            AddPropToElement("_name", serializedConditionalEffect, generalSettingsBox, evt =>
            {
                headerLabel.text = GetHeaderText(conditionalEffectsSO);
                renameAssetButton.visible = conditionalEffectsSO.name != conditionalEffectsSO.Name &&
                                            !conditionalEffectsSO.Name.IsNullOrWhitespace();
                _conditionsListView.Reload();
            });

            var preConditionsBox = CreateBox("Pre-conditions", settingsWrapper,
                helpUrl: "https://spam.infinitevoid.games/conditional-effects.html#pre-conditions");
            var preconditionsList =
                new PreconditionsList.PreconditionsList(conditionalEffectsSO, serializedConditionalEffect);
            preconditionsList.PreconditionChanged += () => SetDescription(conditionalEffectsSO);
            preconditionsList.PreconditionRemoved += () => SetDescription(conditionalEffectsSO);
            preConditionsBox.Add(preconditionsList);

            var effectsBox = CreateBox("Effects", settingsWrapper,
                helpUrl: "https://spam.infinitevoid.games/conditional-effects.html#effects");
            var effectsList = new EffectsList(conditionalEffectsSO, serializedConditionalEffect);
            effectsList.EffectChanged += () => SetDescription(conditionalEffectsSO);
            effectsList.EffectRemoved += () => SetDescription(conditionalEffectsSO);
            effectsBox.Add(effectsList);
        }

        private void SetDescription(ConditionalEffectsSO conditionalEffectsSO)
        {
            var descriptionLabel = rootVisualElement.Q<Label>("conditional-effect-desc");
            descriptionLabel.text = conditionalEffectsSO ? conditionalEffectsSO.ToString() : string.Empty;
        }


        private string GetHeaderText(ConditionalEffectsSO conditionalEffectsSO)
        {
            if (string.IsNullOrWhiteSpace(conditionalEffectsSO.Name))
                return conditionalEffectsSO.name;

            return conditionalEffectsSO.Name.ToLower() != conditionalEffectsSO.name.ToLower()
                ? $"{conditionalEffectsSO.Name} (asset: {conditionalEffectsSO.name})"
                : conditionalEffectsSO.Name;
        }

        private static IList<ConditionalEffectsSO> GetAllConditionalEffects()
        {
            return AssetDatabase
                .FindAssets("t:ConditionalEffectsSO", null)
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<ConditionalEffectsSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(x => x.Name)
                .ToArray();
        }
    }
}