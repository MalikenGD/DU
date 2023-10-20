using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AbilityBase), true)]
    [CanEditMultipleObjects]
    public class AbilityBaseEditor : UnityEditor.Editor
    {
        ProjectileAbilitySO _selectedAbility;

        public override void OnInspectorGUI()
        {
            var t = (AbilityBase)target;
            var serAbility = new SerializedObject(target);
            var abilityName = GetAbilityName(t, serAbility);

            var style = EditorUtils.BackgroundStyle.Get(new Color(.15f, .15f, .15f));
            style.fontSize = 14;
            style.normal.textColor = new Color(0, 255, 255, 1);
            style.alignment = TextAnchor.MiddleCenter;
            style.font = Font.CreateDynamicFontFromOSFont("sans-serif", 14);
            style.padding = new RectOffset(10, 10, 10, 10);

            GUILayout.Box(abilityName, style);
            DrawDefaultInspector();
            GUILayout.Space(20);
            var invokerProp = serAbility.FindProperty("_invoker");
            if (invokerProp.objectReferenceValue == null)
                if (GUILayout.Button("Get invoker from children or parent(s)"))
                {
                    t.GetInvokerFromHierarchy();
                }


            if (t is ProjectileAbility projectileAbility)
                AddProjectileAbilityFields(serAbility, projectileAbility);
        }

        private string GetAbilityName(AbilityBase ability, SerializedObject serAbility)
        {
            if (ability is ProjectileAbility projectileAbility)
            {
                if (projectileAbility.CurrentAbility)
                    return projectileAbility.Name.ToUpper();

                var poolProp = serAbility.FindProperty("_projectilePool");
                if (poolProp.objectReferenceValue == null) return "PROJECTILE ABILITY NOT SET";

                var pool = (ProjectilePool)poolProp.objectReferenceValue;
                var abilityProp = serAbility.FindProperty("_projectileAbility");
                abilityProp.objectReferenceValue = pool.ProjectileAbilitySo;
                serAbility.ApplyModifiedPropertiesWithoutUndo();
                return pool.ProjectileAbilitySo.Name.ToUpper();
            }

            if (!ability) return "MISSING ABILITY";
            return string.IsNullOrWhiteSpace(ability.Name) ? ability.name.ToUpper() : ability.Name.ToUpper();
        }

        private void AddProjectileAbilityFields(SerializedObject serAbilityComponent,
            ProjectileAbility projectileAbility)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying) return;
#endif
            
            var poolProp = serAbilityComponent.FindProperty("_projectilePool");
            var selectedAbility = projectileAbility.CurrentAbility;
            if (!poolProp.objectReferenceValue)
            {
                ShowCreateOrGetPoolButton();
                return;
            }
            var pool = (ProjectilePool)poolProp.objectReferenceValue;
            var projectileInPool = pool.ProjectileAbilitySo;
            if (projectileInPool != selectedAbility)
            {
                poolProp.objectReferenceValue = null;
                serAbilityComponent.ApplyModifiedPropertiesWithoutUndo();
                SpamLogger.EditorOnlyWarning($"The assigned pool was for another ability. The reference was cleared. You can assign a pool that holds projectiles for {projectileAbility.Name}, leave the pool empty, or click 'Find or create projectile pool'.");
                ShowCreateOrGetPoolButton();
                return;
            }


            void ShowCreateOrGetPoolButton()
            {
                EditorGUILayout.LabelField("Pool utils", EditorStyles.boldLabel);
                
                EditorGUILayout.LabelField("If this object is instantiated and active in the scene, you can pre-resolve a pool for the ability by clicking the button below. If it is a prefab, a pool will be automatically resolved at Start.", EditorStyles.wordWrappedLabel);
                
                if (GUILayout.Button("Find or create projectile pool"))
                {
                    var ability = projectileAbility.CurrentAbility;
                    if (!ability)
                    {
                        EditorUtility.DisplayDialog("No ability selected",
                            "You must set an ability before trying to resolve a pool", "Ok");
                        return;
                    }

                    var pool = EditorUtils.GetOrCreatePool(ability);
                    poolProp.objectReferenceValue = pool;
                    serAbilityComponent.ApplyModifiedProperties();
                }
            }
        }
    }
}