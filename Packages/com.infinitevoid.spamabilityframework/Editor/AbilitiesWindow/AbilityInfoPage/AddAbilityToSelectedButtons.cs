using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow.AbilityInfoPage
{
    public class AddAbilityToSelectedButtons : VisualElement
    {
        public AddAbilityToSelectedButtons(AbilityBaseSO ability)
        {
            var wrapper = new Box();
            wrapper.AddToClassList("c-add-ability-wrapper");
            this.Add(wrapper);
            var asChildToggle = new Toggle("Add as child")
            {
                name = "add-as-child-cb",
                tooltip = "Adds the ability as a child to the selected gameobject, instead of directly to the selected gameobject"
            };
            asChildToggle.value = true;
            asChildToggle.AddToClassList("u-margin-bottom");
            wrapper.Add(asChildToggle);
            var firstRow = new Box();
            firstRow.AddToClassList("c-add-ability-box");
            firstRow.AddToClassList("row");
            wrapper.Add(firstRow);
            var secondRow = new Box();
            secondRow.AddToClassList("c-add-ability-box");
            secondRow.AddToClassList("row");
            wrapper.Add(secondRow);
            

            if (ability is ProjectileAbilitySO projectileAbilitySO)
            {
                var addAsTargetedButton = new Button();
                addAsTargetedButton.name = "add-as-targeted-btn";
                addAsTargetedButton.clicked +=
                    () => AddAsPooledToSelected<TargetedProjectileAbility>(projectileAbilitySO);
                addAsTargetedButton.text = "As targeted projectile";
                firstRow.Add(addAsTargetedButton);
                var addAsDirectionButton = new Button();
                addAsDirectionButton.clicked += () =>
                    AddAsPooledToSelected<DirectionalProjectileAbility>(projectileAbilitySO);
                addAsDirectionButton.text = "As directional projectile";
                firstRow.Add(addAsDirectionButton);
            }
            else if (ability is TargetedAbilitySO)
            {
                var addAsTargetedButton = new Button(AddComponentToSelected<TargetedAbility>);
                addAsTargetedButton.text = "As targeted ability";
                firstRow.Add(addAsTargetedButton);
            }
            else if (ability is DirectionalAbilitySO)
            {
                var addAsTargetedButton = new Button(AddComponentToSelected<DirectionalAbility>);
                addAsTargetedButton.text = "As directional ability";
                firstRow.Add(addAsTargetedButton);
            }
            else if (ability is RaycastAbilitySO)
            {
                var addAsTargetedButton = new Button(AddComponentToSelected<RaycastAbility>);
                addAsTargetedButton.text = "As raycast ability";
                firstRow.Add(addAsTargetedButton);
            }

            void AddComponentToSelected<T>() where T : AbilityBase
            {
                var targetGo = GetGameObjectTarget(ability.Name);
                if (targetGo == null) return;

                var abilityComponent = targetGo.AddComponent<T>();
                abilityComponent.GetInvokerFromHierarchy();
                var serializedAbility = new SerializedObject(abilityComponent);
                serializedAbility.Update();
                SerializedProperty serializedProp;
                if (abilityComponent is DirectionalAbility)
                    serializedProp = serializedAbility.FindProperty("_directionalAbility");
                else if (abilityComponent is TargetedAbility)
                    serializedProp = serializedAbility.FindProperty("_targetedAbility");
                else
                    serializedProp = serializedAbility.FindProperty("_raycastAbility");

                serializedProp.objectReferenceValue = ability;
                serializedAbility.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }
        
        private void AddAsPooledToSelected<T>(ProjectileAbilitySO projectileAbilitySO) where T : ProjectileAbility
        {
            var targetGo = GetGameObjectTarget(projectileAbilitySO.Name);
            if (targetGo == null) return;
            if (!projectileAbilitySO.Prefab)
            {
                SpamLogger.EditorOnlyErrorLog(true, "Can't add projectile ability: No projectile assigned. Assign a projectile prefab to the ability before trying to add it to a caster");
                return;
            }
            var pool = EditorUtils.GetOrCreatePool(projectileAbilitySO);
            var serializedPool = new SerializedObject(pool);
            var serializedProp = serializedPool.FindProperty("_projectileAbility");
            serializedProp.objectReferenceValue = projectileAbilitySO;
            serializedPool.ApplyModifiedProperties();

            var abilityComponent = targetGo.AddComponent<T>();
            abilityComponent.GetInvokerFromHierarchy();
            var serializedAbility = new SerializedObject(abilityComponent);
            serializedAbility.Update();
            serializedProp = serializedAbility.FindProperty("_projectilePool");
            serializedProp.objectReferenceValue = pool;
            serializedProp = serializedAbility.FindProperty("_projectileAbility");
            serializedProp.objectReferenceValue = projectileAbilitySO;
            serializedAbility.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private GameObject GetGameObjectTarget(string abilityName)
        {
            
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.Log("No object selected");
                return null;
            }

            if (AddAsChild())
            {
                var child = new GameObject();
                child.transform.parent = selected.transform;
                child.name = abilityName;
                selected = child;
            }

            return selected;
        }
        
        private bool AddAsChild() => this.Q<Toggle>("add-as-child-cb").value;
    }
}