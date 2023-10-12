using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
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
            var t = (AbilityBase) target;
            var serAbility = new SerializedObject(target);
            var abilityName = GetAbilityName(t, serAbility);
            
            var style = EditorUtils.BackgroundStyle.Get(new Color(.15f,.15f,.15f));
            style.fontSize = 14;
            style.normal.textColor = new Color(0, 255, 255, 1);
            style.alignment = TextAnchor.MiddleCenter;
            style.font = Font.CreateDynamicFontFromOSFont("sans-serif", 14);
            style.padding = new RectOffset(10, 10, 10, 10);
            
            GUILayout.Box(abilityName, style);
            DrawDefaultInspector();
            GUILayout.Space(20);
            var invokerProp = serAbility.FindProperty("_invoker");
            if(invokerProp.objectReferenceValue == null)
                if (GUILayout.Button("Get invoker from children or parent(s)"))
                {
                    t.GetInvokerFromHierarchy();
                }

            
            if (t is ProjectileAbility projectileAbility)
                AddProjectileAbilityFields(serAbility, projectileAbility);
            
            
        }

        private string GetAbilityName(AbilityBase ability, SerializedObject serAbility)
        {
            if (ability is ProjectileAbility)
            {
                var poolProp = serAbility.FindProperty("_projectilePool");
                if (poolProp.objectReferenceValue == null) return "PROJECTILE ABILITY NOT SET";
                
                var pool = (ProjectilePool)poolProp.objectReferenceValue;
                return pool.ProjectileAbilitySo.Name.ToUpper();
            }

            return ability.Name.ToUpper();

        }

        private void AddProjectileAbilityFields(SerializedObject serAbilityComponent, ProjectileAbility projectileAbilityComponent)
        {
            var poolProp = serAbilityComponent.FindProperty("_projectilePool");
            // var projectileAbilitySO = (ProjectileAbilitySO)serAbilityComponent.FindProperty("_currentAbility").objectReferenceValue;
            if (poolProp.objectReferenceValue != null)
                return;

            EditorGUILayout.LabelField("Set projectile ability", EditorStyles.boldLabel);
            _selectedAbility =
                EditorGUILayout.ObjectField("Projectile ability", _selectedAbility, typeof(ProjectileAbilitySO), false) as
                    ProjectileAbilitySO;
            if (GUILayout.Button("Find or create projectile pool"))
            {
                if (_selectedAbility == null)
                {
                    EditorUtility.DisplayDialog("No ability selected", "You must set an ability before creating a pool", "Ok");
                    return;
                }

                var pool = EditorUtils.GetPool(_selectedAbility);
                poolProp.objectReferenceValue = pool;
                serAbilityComponent.ApplyModifiedProperties();
            }
        }
    }
}