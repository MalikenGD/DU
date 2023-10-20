using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow
{
    public class AbilitiesWindow : EditorWindow
    {
        private static List<AbilityBaseSO> _allAbilities;
        private static List<AbilityBaseSO> _currentAbilitySet;
        private ListView _abilitiesListView;
        private Box _contentBox;

        public Button CreateNewAbilityButton => this.rootVisualElement.Q<Button>("create-new-ability");
        

        [MenuItem("Window/SPAM Framework/All windows", false, 74)]
        [MenuItem("Tools/SPAM Framework/All windows", false, 50)]
        public static void ShowAllWindows()
        {
            AbilitiesWindow abilitiesWindow = GetWindow<AbilitiesWindow>("Abilities", true);
            abilitiesWindow.minSize = new Vector2(400, 400);
            abilitiesWindow.titleContent = EditorUtils.GetSPAMWindowTitle("Abilities");
            abilitiesWindow.Show();
            var effectsWindow = GetWindow<EffectsWindow.EffectsWindow>("Effects", typeof(AbilitiesWindow));
            effectsWindow.titleContent = EditorUtils.GetSPAMWindowTitle("Effects");
            
            var conditionsWindow = GetWindow<ConditionsWindow.ConditionsWindow>("Conditions", typeof(AbilitiesWindow));
            conditionsWindow.titleContent = EditorUtils.GetSPAMWindowTitle("Conditions");
            
            var conditionalEffectsWindow = GetWindow<ConditionalEffectsWindow.ConditionalEffectsWindow>("Conditional effects", typeof(AbilitiesWindow));
            conditionalEffectsWindow.titleContent = EditorUtils.GetSPAMWindowTitle("Conditional Effects");
            
            var poolingWindow = GetWindow<PoolingWindow.PoolingWindow>("Pooling", typeof(AbilitiesWindow));
            poolingWindow.titleContent = EditorUtils.GetSPAMWindowTitle("Pooling");
        }

        [MenuItem("Tools/SPAM Framework/Abilities", false, 75)]
        public static void ShowWindow()
        {
            AbilitiesWindow wnd = GetWindow<AbilitiesWindow>("Abilities");
            wnd.minSize = new Vector2(300, 300);
            wnd.titleContent = EditorUtils.GetSPAMWindowTitle("Abilities");
            wnd.Show();
        }

        private void OnFocus()
        {
            var searchbutton = rootVisualElement.Q<TextField>("search");
            if (searchbutton == null) return;
            var input = searchbutton.Q<VisualElement>("unity-text-input");
            input.Focus();
            searchbutton.SelectAll();
        }

        public void CreateGUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/AbilitiesWindow.uxml");
            VisualElement tree = visualTree.Instantiate();
            tree.StretchToParentSize();
            rootVisualElement.Add(tree);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/AbilitiesWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/CreateNewAbilityPage.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _contentBox = rootVisualElement.Q<Box>(className: "c-content-container");

            _allAbilities = GetAllAbilities();
            FixIdsIfNotSet();
            _currentAbilitySet = _allAbilities;
            var unknownIcon = AssetDatabase.LoadAssetAtPath<Texture>($"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Textures/unknown.png");
            _abilitiesListView = rootVisualElement.Q<ListView>(className: "c-abilities-list-view");

            var searchbutton = rootVisualElement.Q<TextField>("search");
            searchbutton.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    _currentAbilitySet = _allAbilities;
                }
                else
                {
                    var searchText = evt.newValue.ToLower();
                    _currentAbilitySet = _allAbilities.Where(ab => ab.Name.ToLower().Contains(searchText) || ab.name.ToLower().Contains(searchText)).ToList();
                    
                }

                _abilitiesListView.itemsSource = _currentAbilitySet;
                _abilitiesListView.Reload();
            });

            // _abilitiesListView.styleSheets.Add(styleSheet);
            _abilitiesListView.makeItem = () =>
            {
                var box = new Box();
                var image = new Image();
                var label = new Label();
                box.Add(image);
                box.Add(label);
                return box;
            };
            _abilitiesListView.bindItem = (e, i) =>
            {
                var box = e as Box;
                var label = box.Q<Label>();
                label.text = string.IsNullOrWhiteSpace(_currentAbilitySet[i].Name)
                    ? _currentAbilitySet[i].name
                    : _currentAbilitySet[i].Name;
                var image = box.Q<Image>();
                if (_currentAbilitySet[i].Icon)
                    image.image = _currentAbilitySet[i].Icon.texture;
                else image.image = unknownIcon;
            };
            _abilitiesListView.itemsSource = _currentAbilitySet.ToList();
            _abilitiesListView.selectionType = SelectionType.Single;
            _abilitiesListView.SetOnSelectionChanged(OnAbilitySelected);

            var createNewButton = rootVisualElement.Q<Button>("create-new-ability");
            createNewButton.clicked += SwitchToCreateNewAbilityView;
        }

        public static void ReloadAbilities()
        {
            _allAbilities = GetAllAbilities();
            FixIdsIfNotSet();
            _currentAbilitySet = _allAbilities;
            AbilitiesWindow wnd = GetWindow<AbilitiesWindow>();
            var listView = wnd.rootVisualElement.Q<ListView>(className: "c-abilities-list-view");
            listView.itemsSource = _currentAbilitySet.ToList();
            listView.Reload();
            listView.SetSelection(0);
        }

        private static void FixIdsIfNotSet()
        {
            var anyAbilityMissingId = _allAbilities.FirstOrDefault(x => x.Id == 0);
            var anyProjectileMissingId = _allAbilities
                .Select(x => x as ProjectileAbilitySO)
                .Where(x => x != null)
                .FirstOrDefault(x => x.Prefab && x.Prefab.Id == 0);
            if (!anyAbilityMissingId && !anyProjectileMissingId) return;
            
            SpamLogger.EditorOnlyLog("Found abilities or projectiles without IDs. Fixing...");
            _allAbilities = GetAllAbilities();
            int projectileIndex = 0;
            for (var index = 0; index < _allAbilities.Count; index++)
            {
                _allAbilities[index].SetId(index+1);
                EditorUtility.SetDirty(_allAbilities[index]);
                if(_allAbilities[index] is ProjectileAbilitySO projectileAbilitySO && projectileAbilitySO.Prefab)
                {
                    projectileAbilitySO.Prefab.SetId(++projectileIndex);
                    EditorUtility.SetDirty(projectileAbilitySO.Prefab);
                }
            }
            AssetDatabase.SaveAssets();
            SpamLogger.EditorOnlyLog("Assigned IDs of abilities - DONE.");
        }

        public static void SetSelectionAsSelected()
        {
            AbilitiesWindow wnd = GetWindow<AbilitiesWindow>();
            var selectedIndex = _currentAbilitySet.IndexOf(Selection.activeObject as AbilityBaseSO);
            var listView = wnd.rootVisualElement.Q<ListView>(className: "c-abilities-list-view");
            listView.ClearSelection();
            listView.SetSelection(selectedIndex);
        }

        private void SwitchToCreateNewAbilityView()
        {
            _contentBox.Clear();
            _contentBox.Add(new CreateNewAbilityPage(_contentBox));
        }

        public static List<AbilityBaseSO> GetAllAbilities()
        {
            return AssetDatabase
                .FindAssets("t:AbilityBaseSO", null)
                .Select(guid => AssetDatabase.LoadAssetAtPath<AbilityBaseSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(x => x.Name)
                .ToList();
        }


        private void OnAbilitySelected(IEnumerable<object> objects)
        {
            foreach (var item in objects)
            {
                _contentBox.Clear();

                var ability = (AbilityBaseSO) item;
                if (ability == null)
                    return;
                _contentBox.Add(new AbilityInfoPage.AbilityInfoPage(ability, _contentBox, _abilitiesListView.Reload));
            }
        }
    }
}