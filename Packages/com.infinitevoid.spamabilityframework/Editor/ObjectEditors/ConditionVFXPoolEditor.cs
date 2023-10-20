using InfiniteVoid.SpamFramework.Core;
using InfiniteVoid.SpamFramework.Core.Components.Conditions;
using InfiniteVoid.SpamFramework.Core.Effects;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(ConditionVFXPool), true)]
    [CanEditMultipleObjects]
    public class ConditionVFXPoolEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            var item = (ConditionVFXPool)target;
            if (!item.Condition) return;
            EditorGUILayout.LabelField(new GUIContent("Num in pool","This value is set on the condition."), new GUIContent(item.Condition.NumPooled.ToString()));
        }
    }
}