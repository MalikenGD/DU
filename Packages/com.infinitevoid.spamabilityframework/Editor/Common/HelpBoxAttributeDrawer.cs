using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.Common
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : PropertyDrawer
    {
        private float _baseHeight;
        private HelpBoxAttribute _helpBoxAttribute => (HelpBoxAttribute)attribute;

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            // Store the original property height for later use...
            _baseHeight = base.GetPropertyHeight(prop, label);

            float minHeight = _helpBoxAttribute.messageType != HelpBoxMessageType.None
             ? 8 * 5 : 5;

            var content = new GUIContent(_helpBoxAttribute.text);
            var style = GUI.skin.GetStyle("helpbox");

            var height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);

            // Add tiny padding to not overflow container
            height += 2 * 2;

            return height > minHeight 
                ? height + _baseHeight 
                : minHeight + _baseHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var helpBoxAttribute = attribute as HelpBoxAttribute;
            if (helpBoxAttribute == null) return;
            
            var helpPos = position;
            const int margin = 2;
            helpPos.height -= _baseHeight + margin;
            position.y += helpPos.height + margin;
            position.height = _baseHeight;
            EditorGUI.BeginProperty(position, label, prop);
            EditorGUI.HelpBox(helpPos, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
            EditorGUI.PropertyField(position, prop, label);
            EditorGUI.EndProperty();
        }

        private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
        {
            switch (helpBoxMessageType)
            {
                default:
                case HelpBoxMessageType.None: return MessageType.None;
                case HelpBoxMessageType.Info: return MessageType.Info;
                case HelpBoxMessageType.Warning: return MessageType.Warning;
                case HelpBoxMessageType.Error: return MessageType.Error;
            }
        }
    }
}