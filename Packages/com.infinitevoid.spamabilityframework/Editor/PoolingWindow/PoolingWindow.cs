using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.PoolingWindow
{
    public class PoolingWindow : EditorWindow
    {
        private static List<AbilityBaseSO> _abilitiesWithPools;
        private static PoolingSettingsSO _settingsObject;
        private static TemplateContainer _tree;
        private static VisualElement _vfxPoolsContainer;
        private static VisualElement _projectilePoolsContainer;
        private const string VFX_POOLS_CONTAINER_NAME = "vfx-pools-container";
        private const string PROJECTILE_POOLS_CONTAINER_NAME = "projectiles-pools-container";


        [MenuItem("Tools/SPAM Framework/Pooling", false, 90)]
        public static void ShowWindow()
        {
            PoolingWindow wnd = GetWindow<PoolingWindow>("Pooling");
            wnd.minSize = new Vector2(400, 400);
            wnd.titleContent = EditorUtils.GetSPAMWindowTitle("Pooling");
            _vfxPoolsContainer = wnd.rootVisualElement.Q<VisualElement>(VFX_POOLS_CONTAINER_NAME);
            _projectilePoolsContainer = wnd.rootVisualElement.Q<VisualElement>(PROJECTILE_POOLS_CONTAINER_NAME);
            ReloadPools();
            wnd.Show();
        }


        public static void ReloadPools()
        {
            GetOrCreateSettingsObject();
            var allAbilities = AbilitiesWindow.AbilitiesWindow.GetAllAbilities();
            ReloadPooledVFX(allAbilities);
            ReloadPooledProjectiles(allAbilities);
        }

        public static void ReloadPooledProjectiles(List<AbilityBaseSO> allAbilities)
        {
            var projectileAbililies = allAbilities.OfType<ProjectileAbilitySO>().ToArray();

            _settingsObject.AddProjectiles(projectileAbililies);

            AssetDatabase.SaveAssets();
            if (_projectilePoolsContainer == null) return;
            _projectilePoolsContainer.Clear();
            var projectilePoolsInScene = FindObjectsOfType<ProjectilePool>();
            var serializedSettings = new SerializedObject(_settingsObject);
            for (int i = 0; i < _settingsObject.PooledProjectiles.Count; i++)
            {
                var row = CreateProjectileRow(i, serializedSettings, projectilePoolsInScene, projectileAbililies);
                _projectilePoolsContainer.Add(row);
            }
        }

        private static VisualElement CreateProjectileRow(int i, SerializedObject serializedSettings,
            IEnumerable<ProjectilePool> projectilePoolsInScene, ProjectileAbilitySO[] allProjectileAbilities)
        {
            var projectileAbility =
                allProjectileAbilities.First(x => x.Id == _settingsObject.PooledProjectiles[i].ProjectileAbilityId);
            var usedIn = projectileAbility.Name;

            return new PooledProjectileRowItem(
                projectileAbility,
                serializedSettings.FindProperty("PooledProjectiles").GetArrayElementAtIndex(i)
                    .FindPropertyRelative("NumInstances"),
                serializedSettings,
                usedIn,
                projectilePoolsInScene.ToList());
        }

        private static void ReloadPooledVFX(List<AbilityBaseSO> allAbilities)
        {
            var abilitiesWithPooledVfx = allAbilities.Where(x => x.HasPooledVfx()).ToArray();
            _settingsObject.AddVFXs(abilitiesWithPooledVfx);

            AssetDatabase.SaveAssets();
            if (_vfxPoolsContainer == null) return;
            _vfxPoolsContainer.Clear();
            var serializedSettings = new SerializedObject(_settingsObject);
            for (int i = 0; i < _settingsObject.PooledVfx.Count; i++)
            {
                if (!_settingsObject.PooledVfx[i].Prefab) continue;
                // var boxWrapper = CreateVfxBox(i, serializedSettings, abilitiesWithPooledVfx);
                // _poolsContainer.Add(boxWrapper);
                var row = CreateVfxRow(i, serializedSettings, abilitiesWithPooledVfx);
                _vfxPoolsContainer.Add(row);
            }
        }

        private static VisualElement CreateVfxRow(int i, SerializedObject serializedSettings,
            AbilityBaseSO[] abilitiesWithPooledVfx)
        {
            var abilitiesUsingVfx = abilitiesWithPooledVfx.Where(x =>
                x.VFX.OnHitVfx == _settingsObject.PooledVfx[i].Prefab).Select(x => x.Name).ToArray();

            var usedIn = $"{string.Join(", ", abilitiesUsingVfx)}";

            return new PooledVfxRowItem(
                _settingsObject.PooledVfx[i].Prefab.name,
                serializedSettings.FindProperty("PooledVfx").GetArrayElementAtIndex(i)
                    .FindPropertyRelative("NumInstances"),
                serializedSettings,
                usedIn,
                abilitiesWithPooledVfx.All(x => x.VFX.OnHitVfx != _settingsObject.PooledVfx[i].Prefab),
                () =>
                {
                    _settingsObject.PooledVfx.RemoveAt(i);
                    ReloadPools();
                });
        }


        public void CreateGUI()
        {
            _tree = SetupVisualTree();
            _vfxPoolsContainer = _tree.Q<VisualElement>(VFX_POOLS_CONTAINER_NAME);
            _projectilePoolsContainer = _tree.Q<VisualElement>(PROJECTILE_POOLS_CONTAINER_NAME);
            ReloadPools();
        }

        private static void GetOrCreateSettingsObject()
        {
            AssetDatabase.RefreshSettings();
            _settingsObject = Resources.Load<PoolingSettingsSO>(SpamResources.POOL_SETTINGS);
            if (_settingsObject) return;

            var assetPath = Common.EditorUtils.CreateAndGetAssetPath(SpamResources.POOL_SETTINGS_PATH);
            var instance = CreateInstance<PoolingSettingsSO>();
            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.SaveAssets();
            _settingsObject = instance;
        }

        private TemplateContainer SetupVisualTree()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/PoolingWindow/PoolingWindow.uxml");

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
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/PoolingWindow/PoolingWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            tree.Q<Button>("refresh-pool-state-btn").clicked += () =>
            {
                ReloadPools();
                var poolsInScene = GameObject.FindObjectsOfType<ProjectilePool>();
                tree.Query<PooledProjectileRowItem>().ForEach(x =>
                {
                    if (x == null) return;
                    x.SetButtonStates(poolsInScene);
                });
                SpamLogger.EditorOnlyLog("Refreshed pool state");
            };
            
            return tree;
        }
    }
}