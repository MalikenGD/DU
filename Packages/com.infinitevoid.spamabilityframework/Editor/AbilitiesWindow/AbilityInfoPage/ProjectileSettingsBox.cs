using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow.AbilityInfoPage
{
    public class ProjectileSettingsBox : VisualElement
    {
        public ProjectileSettingsBox(Box settingsWrapper, SerializedObject serializedAbility, ProjectileAbilitySO projectileAbilitySo)
        {
            var projectileSettingsBox = CommonUI.CreateBox("Projectile settings", settingsWrapper, helpUrl:"https://spam.infinitevoid.games/projectile-ability.html");
            var projectileWrapper = new VisualElement();
            projectileWrapper.AddToClassList("c-projectile-wrapper");
            var objectPreview = new VisualElement();
            objectPreview.AddToClassList("c-projectile-wrapper__preview");
            var texture = new StyleBackground(AssetPreview.GetAssetPreview(projectileAbilitySo.Prefab != null ? projectileAbilitySo.Prefab.gameObject : null));
            objectPreview.style.backgroundImage = texture;
            CommonUI.AddPropToElement("_projectilePrefab", serializedAbility, projectileWrapper, additionalClass: "c-projectile-wrapper__selector", onChange:
                evt =>
                {
                    if(evt.changedProperty.objectReferenceValue == null)
                        objectPreview.style.backgroundImage = null;
                        
                    var newTexture = new StyleBackground(AssetPreview.GetAssetPreview(projectileAbilitySo.Prefab != null ? projectileAbilitySo.Prefab.gameObject : null));
                    objectPreview.style.backgroundImage = newTexture;
                });
            projectileWrapper.Add(objectPreview);
            projectileSettingsBox.Add(projectileWrapper);

            CommonUI.AddPropToElement("_timeToLive", serializedAbility, projectileSettingsBox);
            CommonUI.AddPropToElement("_inFlightSound", serializedAbility, projectileSettingsBox);
            projectileSettingsBox.Add(CommonUI.CreateHelpBox(
                "Disable: Stop movement and disable collider. Deactivate: Completely deactivate the projectile",
                "hit-action-help"));
            CommonUI.AddPropToElement("_onHitAction", serializedAbility, projectileSettingsBox);

            var castOnGroundSettings = new VisualElement { name = "cast-on-ground-wrapper" };
            castOnGroundSettings.Add(CommonUI.CreateHelpBox(
                "When cast on ground this distance will be used to check if the projectile has reached it's target.",
                "distance-check-help"));
            CommonUI.AddPropToElement("_distanceCheckRange", serializedAbility, castOnGroundSettings);
            projectileSettingsBox.Add(castOnGroundSettings);
            projectileSettingsBox.Add(CommonUI.CreateHeader3("Spawned projectiles", "u-margin-top"));
            projectileSettingsBox.Add(new ProjectileSpawnList.ProjectileSpawnList(projectileAbilitySo, serializedAbility));
        }
    }
}