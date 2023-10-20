using System.IO;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Projectiles;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow.ProjectileSpawnList
{
    public class ProjectileSpawnList : VisualElement
    {
        private const string MOVEMENT_FIELD_NAME = "movement";
        public ProjectileSpawnList(ProjectileAbilitySO ability, SerializedObject serializedAbility)
        {
            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/ProjectileSpawnList/ProjectileSpawnList.uxml");
            visualTree.CloneTree(this);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/ProjectileSpawnList/ProjectileSpawnList.uss");
            this.styleSheets.Add(styleSheet);
            
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            this.styleSheets.Add(styleSheet);

            var listview = this.Q<ListView>("proj-spawn-list-view");
            var prop = serializedAbility.FindProperty("_spawnedProjectiles");
            listview.itemsSource = ability.SpawnedProjectiles;
            listview.selectionType = SelectionType.Single;
            listview.reorderable = true;
            listview.makeItem = MakeProjectileSpawnSettingsListItem;
            listview.SetHeight(ability.SpawnedProjectiles.Count);
            listview.bindItem = (e, i) =>
            {
                serializedAbility.Update();
                var container = (Box)e;
                var spanwedProjectileProp = prop.GetArrayElementAtIndex(i);
                SetupSpawnTimeField(spanwedProjectileProp, container);
                SetupMovementObjectField(spanwedProjectileProp, container, i);
                // SetupNameAndDescription(container, spawnedProjectile);
                // SetupOnCasterField(effectAndTimeProp, container, i);
                
                container.Q<Button>("remove-projectile").clicked += () =>
                {
                    ability.SpawnedProjectiles.RemoveAt(i);
                    serializedAbility.Update();
                    listview.SetHeight(ability.SpawnedProjectiles.Count);
                    listview.Reload();
                };

                container.Q<Button>("create-new-movement").clicked += () =>
                {
                    var movementAsset = ScriptableObject.CreateInstance<ProjectileMovementBehaviourSO>();
                    var path = AssetDatabase.GetAssetPath(serializedAbility.targetObject);
                    var directory = Path.GetDirectoryName(path);
                    AssetDatabase.CreateAsset(movementAsset,
                        Path.Combine(directory,
                            $"{serializedAbility.FindProperty("_name").stringValue}_{i.ToString()}_movement.asset"));
                    var movementProp = spanwedProjectileProp.FindPropertyRelative("MovementBehaviour");
                    movementProp.objectReferenceValue = movementAsset;
                    serializedAbility.ApplyModifiedProperties();
                    Selection.activeObject = movementAsset;
                };
            };

            this.Q<Button>("add-projectile-button").clicked += () =>
            {
                ability.SpawnedProjectiles.Add(new ProjectileSettings());
                prop.isExpanded = true;
                serializedAbility.Update();
                listview.SetHeight(ability.SpawnedProjectiles.Count);
                listview.Reload();
            };
            
            void SetupSpawnTimeField(SerializedProperty projectileSpawnSettingsProp, Box container)
            {
                var spawnTimeField = container.Q<PropertyField>();
                spawnTimeField.BindProperty(projectileSpawnSettingsProp.FindPropertyRelative(nameof(ProjectileSettings.SpawnTime)));
            }
            
            void SetupMovementObjectField(SerializedProperty projectileSpawnSettingsProp, Box container, int i)
            {
                var movementField = container.Q<ObjectField>(MOVEMENT_FIELD_NAME);
                movementField.BindProperty(projectileSpawnSettingsProp.FindPropertyRelative(nameof(ProjectileSettings.MovementBehaviour)));
            }
        }

        private VisualElement MakeProjectileSpawnSettingsListItem()
        {
            var box = new Box();
            var button = new Button {name ="remove-projectile"};
            box.Add(button);
            button.style.width = 20;
            button.AddToClassList("icon-minus");
            box.AddToClassList("c-proj-spawn-list-view__item");

            var spawnTimeField = new PropertyField();
            box.Add(spawnTimeField);
            
            var movementBehaviourObjectField = new ObjectField();
            movementBehaviourObjectField.name = MOVEMENT_FIELD_NAME;
            movementBehaviourObjectField.objectType = typeof(ProjectileMovementBehaviourSO);
            movementBehaviourObjectField.tooltip = "Double click to edit in inspector. To create a new movement, right-click in the project tab and select Create / SPAM Framework / Projectile Movement";
            box.Add(movementBehaviourObjectField);
            
            var newMovementButton = new Button {name ="create-new-movement"};
            box.Add(newMovementButton);
            newMovementButton.style.width = 20;
            newMovementButton.tooltip =
                "Create new projectile movement asset. This will be placed in the same folder as the ability asset.";
            newMovementButton.AddToClassList("icon-plus");


            // var timeField = new FloatField();
            // timeField.tooltip =
            //     "The time it takes for the effect to be applied. Adds a delay between this and the next effect in the list.";
            // box.Add(timeField);

            // var onCasterField = new Toggle();
            // onCasterField.style.marginLeft = 25;
            // onCasterField.style.width = 35;
            // onCasterField.tooltip = "Should the effect be applied to the caster instead of the target?";
            // box.Add(onCasterField);
            //
            // var label = new Label();
            // label.name = "info-text";
            // box.Add(label);
            //
            return box;
        }
    }
}