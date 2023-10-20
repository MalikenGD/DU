using System.Linq;
using InfiniteVoid.SpamFramework.Core;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Conditions;
using InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AbilityConditionsTarget), true)]
    [CanEditMultipleObjects]
    public class AbilityConditionsTargetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var item = (AbilityConditionsTarget)target;
            if(0 < item.ValidConditions.Count && 0 < item.InitialImmunities.Count)
                EditorGUILayout.HelpBox("Immunities will be disregarded since valid conditions have been added.", MessageType.Warning);
            DrawDefaultInspector();
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Active conditions",
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            if (!Application.isPlaying)
                EditorGUILayout.LabelField("Active conditions displayed in Play-mode",
                    new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
            else
                DisplayActiveConditions(item);
            
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Active immunities",
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            if (!Application.isPlaying)
                EditorGUILayout.LabelField("Active immunities displayed in Play-mode",
                    new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
            else
                DisplayActiveImmunities(item);

        }

        private void DisplayActiveImmunities(AbilityConditionsTarget conditionsTarget)
        {
            if (conditionsTarget.ActiveImmunities is null) return;
            int numActiveImmunities = 0;
            foreach (var immunity in conditionsTarget.ActiveImmunities)
            {
                if (!immunity) continue;
                EditorGUILayout.LabelField(immunity.Name);
                numActiveImmunities++;
            }

            if (numActiveImmunities == 0)
                EditorGUILayout.LabelField("No active immunities",
                    new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
        }

        private void DisplayActiveConditions(AbilityConditionsTarget conditionsTarget)
        {
            if (conditionsTarget.ActiveConditions is null) return;
            int numActiveConditions = 0;
            foreach (var activeCondition in conditionsTarget.ActiveConditions)
            {
                if (activeCondition == ActiveCondition.None) continue;
                EditorGUILayout.LabelField(activeCondition.Condition.Name,
                    activeCondition.IsPermanent ? "Permanent" : "Temporary");
                numActiveConditions++;
            }

            if (numActiveConditions == 0)
                EditorGUILayout.LabelField("No active conditions",
                    new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
        }
    }
}