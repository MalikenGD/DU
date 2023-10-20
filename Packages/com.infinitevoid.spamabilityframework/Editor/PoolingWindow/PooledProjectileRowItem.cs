using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Elements;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.PoolingWindow
{
    public class PooledProjectileRowItem : VisualElement
    {
        private const string CREATE_POOL_BUTTON_NAME = "create-pool-button";
        private const string REFRESH_POOL_BUTTON_NAME = "refresh-pool-button";
        private const string SELECT_POOL_BUTTON_NAME = "select-pool-button";
        private ProjectileAbilitySO _projectileAbility;
        private SerializedProperty _countProperty;

        public PooledProjectileRowItem(ProjectileAbilitySO projectileAbility,
            SerializedProperty countProperty,
            SerializedObject serializedSettings,
            string usedIn, 
            List<ProjectilePool> projectilePoolsInScene)
        {
            _projectileAbility = projectileAbility;
            _countProperty = countProperty;
            this.AddToClassList("c-pooled-item--row");

            var projectileName = projectileAbility.Prefab ? projectileAbility.Prefab.name : projectileAbility.Name;
            var iconLabel = new IconLabel(projectileName,
                IconLabel.IconType.Error,
                "Pooled count must be at least 1",
                additionalClass: "c-pooled-item__name");
            this.Add(iconLabel);
            
            var countPropField = new PropertyField(countProperty);
            countPropField.label = string.Empty;
            countPropField.AddToClassList("c-pooled-item__count");
            this.Add(countPropField);

            var button = new Button(() =>
            {
                var go = new GameObject();
                go.name = $"{_projectileAbility.Name}_pool";
                var pool = go.AddComponent<ProjectilePool>();
                pool.SetProjectile(_projectileAbility);
                SetCreatePoolButtonState(false);
                this.Q<Button>(SELECT_POOL_BUTTON_NAME).SetEnabled(true);
                this.Q<Button>(REFRESH_POOL_BUTTON_NAME).SetEnabled(true);
            });
            button.name = CREATE_POOL_BUTTON_NAME;
            button.AddToClassList("icon-plus");
            button.AddToClassList("c-button--icon");
            this.Add(button);

            var refreshPoolsButton = new Button(RefreshPool);
            refreshPoolsButton.name = REFRESH_POOL_BUTTON_NAME;
            refreshPoolsButton.AddToClassList("icon-refresh");
            refreshPoolsButton.AddToClassList("c-button--icon");
            refreshPoolsButton.SetEnabled(false);
            refreshPoolsButton.tooltip = "Pre-fill the pool in the scene with the given amount of projectiles (manual sync)";
            this.Add(refreshPoolsButton);

            var findPoolInSceneButton = new Button(SelectPoolInScene);
            findPoolInSceneButton.name = SELECT_POOL_BUTTON_NAME;
            findPoolInSceneButton.AddToClassList("icon-hierarchy");
            findPoolInSceneButton.style.width = 20;
            findPoolInSceneButton.style.marginRight = 10;
            findPoolInSceneButton.tooltip = "Select pool in Hierarchy";
            findPoolInSceneButton.SetEnabled(projectilePoolsInScene.Any(x => x.ProjectileAbilitySo == _projectileAbility));
            this.Add(findPoolInSceneButton);
                
            
            SetButtonStates(projectilePoolsInScene);
            
            var usedInLabel = new Label(usedIn);
            usedInLabel.tooltip = usedIn;
            usedInLabel.AddToClassList("c-pooled-item__meta");
            this.Add(usedInLabel);
            
            countPropField.RegisterValueChangeCallback(ev =>
            {
                iconLabel.SetIconVisible(ev.changedProperty.intValue == 0);
                SetRefreshButtonState(ev.changedProperty.intValue);
            });
            this.Bind(serializedSettings);
        }

        private static List<ProjectilePool> GetPoolsInScene() => GameObject.FindObjectsOfType<ProjectilePool>().ToList();

        private void SelectPoolInScene()
        {
            var sceneItem = GetPoolsInScene().FirstOrDefault(x => x.ProjectileAbilitySo == _projectileAbility);
            if (!sceneItem)
            {
                SpamLogger.EditorOnlyLog("Pool could not be found or has been destroyed. Reload pools and try again.");
                return;
            }
            Selection.activeObject = sceneItem;
        }

        private void SetRefreshButtonState(int projectilesSettingsCount)
        {
            var pool = GetPoolsInScene().FirstOrDefault(x => x.ProjectileAbilitySo == _projectileAbility);
            if (!pool) return;

            if (!this.Q<Button>(CREATE_POOL_BUTTON_NAME).enabledSelf)
                this.Q<Button>(REFRESH_POOL_BUTTON_NAME).SetEnabled(projectilesSettingsCount != 0 && pool.GetProjectileCount() != projectilesSettingsCount);
        }

        private void RefreshPool()
        {
            var refreshButton = this.Q<Button>(REFRESH_POOL_BUTTON_NAME);
            refreshButton.SetEnabled(false);
            var pool = GetPoolsInScene().FirstOrDefault(x => x.ProjectileAbilitySo == _projectileAbility);
            if (!pool) return;
            
            pool.RefillPool();
        }

        public void SetButtonStates(IEnumerable<ProjectilePool> projectilePoolsInScene)
        {
            SetCreatePoolButtonState(projectilePoolsInScene.All(x => x.ProjectileAbilitySo != _projectileAbility));
            SetRefreshButtonState(_countProperty.intValue);
        }

        private void SetCreatePoolButtonState(bool enabled)
        {
            var poolButton = this.Q<Button>(CREATE_POOL_BUTTON_NAME);
            poolButton.SetEnabled(enabled);
            poolButton.tooltip = poolButton.enabledSelf
                ? "Create pool in current active scene"
                : "Pool already exists in scene";
        }
    }
}