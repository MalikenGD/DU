using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.Utils;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.EffectsWindow
{
    public class EffectsWindow : EditorWindow
    {
        private static IList<AbilityEffectSO> _effects;
        private IEnumerable<AbilityEffectSO> _effectTypes;
        private ListView _effectsListView;

        [MenuItem("Tools/SPAM Framework/Effects", false, 80)]
        public static void ShowWindow()
        {
            EffectsWindow wnd = GetWindow<EffectsWindow>("Effects");
            wnd.minSize = new Vector2(400, 400);
            wnd.Show();
        }

        public static void ReloadEffects()
        {
            _effects = GetAllEffects();
            EffectsWindow wnd = GetWindow<EffectsWindow>();
            wnd.titleContent = new GUIContent("Effects");
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.itemsSource = _effects.ToList();
            listView.Reload();
            listView.SetSelection(0);
        }

        public static void SetSelectionAsSelected()
        {
            EffectsWindow wnd = GetWindow<EffectsWindow>();
            wnd.titleContent = new GUIContent("Effects");
            var selectedIndex = _effects.IndexOf(Selection.activeObject as AbilityEffectSO);
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.ClearSelection();
            listView.SetSelection(selectedIndex);
        }

        public void CreateGUI()
        {
            var tree = SetupVisualTree();

            var createNewButton = tree.Q<Button>("create-new-effect");
            createNewButton.clicked += SwitchToCreateNewEffectPage;

            _effects = GetAllEffects();
            _effectTypes = GetAllEffectTypes();
            _effectsListView.makeItem = () => new Label();
            _effectsListView.bindItem = (e, i) =>
                (e as Label).text = string.IsNullOrWhiteSpace(_effects[i].Name) ? _effects[i].name : _effects[i].Name;
            _effectsListView.itemsSource = _effects.ToList();
            _effectsListView.selectionType = SelectionType.Single;
            _effectsListView.SetOnSelectionChanged(OnEffectSelected);
        }

        private TemplateContainer SetupVisualTree()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectsWindow/EffectsWindow.uxml");

            var tree = visualTree.Instantiate();
            tree.StretchToParentSize();
            rootVisualElement.Add(tree);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectsWindow/EffectsWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _effectsListView = rootVisualElement.Q<ListView>(name: "effects-list");
            _effectsListView.styleSheets.Add(styleSheet);
            return tree;
        }

        public void SwitchToCreateNewEffectPage()
        {
            var effectInfoContainer = rootVisualElement.Q<Box>("effect-info");
            effectInfoContainer.style.display = DisplayStyle.None;
            var newEffectPage = rootVisualElement.Q<Box>("new-effect-page");
            newEffectPage.style.display = DisplayStyle.Flex;
            var newEffectContainer = rootVisualElement.Q<Box>("new-effect");
            newEffectContainer.Clear();
            newEffectContainer.Add(new CreateNewEffectPage(newEffectContainer, _effectTypes));
        }

        private void OnEffectSelected(IEnumerable<object> objects)
        {
            var newEffectPage = rootVisualElement.Q<Box>("new-effect-page");
            newEffectPage.style.display = DisplayStyle.None;
            var effect = objects.FirstOrDefault();
            var effectInfoContainer = rootVisualElement.Q<Box>("effect-info");
            effectInfoContainer.style.display = effect == null ? DisplayStyle.None : DisplayStyle.Flex;
            if (effect == null) // IF this effect object was deleted while editor was open
            {
                ReloadEffects();
                return;
            }

            var effectSO = (AbilityEffectSO)effect;
            if (effectSO == null)
                return;

            var serializedEffect = new SerializedObject(effectSO);
            var header = rootVisualElement.Q<Label>("effect-header");
            header.text
                = GetHeaderText(effectSO);


            var goToAssetButton = rootVisualElement.Q<Button>("select-asset-button");
            goToAssetButton.clicked += () => { Selection.activeObject = effectSO; };

            var renameAssetButton = rootVisualElement.Q<Button>("rename-asset-button");
            renameAssetButton.visible = effectSO.name != effectSO.Name && !effectSO.Name.IsNullOrWhitespace();
            renameAssetButton.clicked += () =>
            {
                var path = AssetDatabase.GetAssetPath(effectSO);
                AssetDatabase.RenameAsset(path, effectSO.Name);
                header.text = GetHeaderText(effectSO);
                renameAssetButton.visible = false;
            };

            var effectType = effectSO.GetType().Name;
            rootVisualElement.Q<Label>("effect-type-name").text = effectType.Remove(effectType.Length - 2);

            var desc = rootVisualElement.Q<TextElement>("effect-desc");
            desc.text = effectSO.HelpDescription;

            var settingsWrapper = rootVisualElement.Q<Box>("effect-settings");
            settingsWrapper.Clear();
            var settingsBox = CreateBox("General settings", settingsWrapper);
            AddPropToElement("_name", serializedEffect, settingsBox, evt =>
            {
                header.text = GetHeaderText(effectSO);
                renameAssetButton.visible = effectSO.name != effectSO.Name && !effectSO.Name.IsNullOrWhitespace();
                _effectsListView.Reload();
            });
            AddPropToElement("_description", serializedEffect, settingsBox, additionalClass: "c-effect-description");
            settingsBox.Add(CreateHelpBox(
                "If this is turned off then the effect won't be included in the list of effects an ability publicly has. Useful for removing an effect from an ability's public interface so the ability's effects can be listed in the GUI.",
                string.Empty));
            AddPropToElement("_includedInAbilityEffects", serializedEffect, settingsBox);

            AddPropsSpecificToEffectType(serializedEffect, effectSO);
        }

        private void AddPropsSpecificToEffectType(SerializedObject serializedEffect, AbilityEffectSO effectSO)
        {
            var specificSettingsWrapper = rootVisualElement.Q<Box>("effect-specific-settings");
            specificSettingsWrapper.Clear();
            var specificSettingsBox = CreateBox("Effect specific settings", specificSettingsWrapper);

            var excludedProperties = new[]
            {
                "m_Script",
                "_name",
                "_description",
                "_includedInAbilityEffects"
            };

            var prop = serializedEffect.GetIterator();
            prop.Next(true);
            while (prop.NextVisible(false))
            {
                if (excludedProperties.Contains(prop.propertyPath))
                    continue;

                // Special case to show a link to the condition
                if (prop.propertyPath == "_condition" &&
                    (effectSO is AddConditionEffectSO || effectSO is RemoveConditionEffectSO))
                {
                    AddReferenceFieldWithEditButton(prop, serializedEffect, specificSettingsBox);
                    continue;
                }
                var field = new PropertyField(prop);
                field.AddToClassList("u-wide-label");

                // Special case since we style PropertyField with a greater height and this breaks
                // lists. audioclips is as array.
                if (prop.propertyPath == "_audioClips")
                    field.AddToClassList("u-list-property");

                field.Bind(serializedEffect);
                specificSettingsBox.Add(field);
            }
        }

        private static void AddReferenceFieldWithEditButton(SerializedProperty prop, SerializedObject serializedEffect, Box parent)
        {
            var wrapper = new Box();
            wrapper.AddToClassList("c-reference-with-edit");
            var field = new PropertyField(prop);
            field.AddToClassList("u-wide-label");

            field.Bind(serializedEffect);
            wrapper.Add(field);

            var editEffectButton = new Button { name = "edit-object" };
            editEffectButton.bindingPath = "_condition";
            editEffectButton.style.width = 20;
            editEffectButton.AddToClassList("icon-settings");
            editEffectButton.clicked += () =>
            {
                var conditionProp = serializedEffect.FindProperty(editEffectButton.bindingPath);
                if (!conditionProp.objectReferenceValue) return;
                Selection.activeObject = conditionProp.objectReferenceValue;
                ConditionsWindow.ConditionsWindow.SetSelectionAsSelected();
                ConditionsWindow.ConditionsWindow.ShowWindow();
            };
            wrapper.Add(editEffectButton);
            parent.Add(wrapper);
        }

        private string GetHeaderText(AbilityEffectSO effectSO)
        {
            if (string.IsNullOrWhiteSpace(effectSO.Name))
                return effectSO.name;

            return effectSO.name == effectSO.Name
                ? effectSO.Name
                : $"{effectSO.Name} (asset: {effectSO.name})";
        }

        private static IList<AbilityEffectSO> GetAllEffects()
        {
            return AssetDatabase
                .FindAssets("t:AbilityEffectSO", null)
                .Select(guid => AssetDatabase.LoadAssetAtPath<AbilityEffectSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        private static IEnumerable<AbilityEffectSO> GetAllEffectTypes()
        {
            return
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(AbilityEffectSO)))
                    .Select(t => (AbilityEffectSO)ScriptableObject.CreateInstance(t));
        }
    }
}