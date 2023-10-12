using InfiniteVoid.SpamFramework.Core.Utils;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomPropertyDrawer(typeof(FloatValue))]
    public class FloatValuePropertyDrawer : PropertyDrawer
    {
        private string[] _popupOptions = { "Constant", "Random between two constants" };
        private const string HELP_BOX_NAME = "float-value-error";
        /// <summary> Cached style to use to draw the popup button. </summary>
        private GUIStyle _popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_popupStyle == null)
            {
                _popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                _popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            // Get properties
            SerializedProperty valueType = property.FindPropertyRelative("Type");
            SerializedProperty minValue = property.FindPropertyRelative("_minValue");
            SerializedProperty maxValue = property.FindPropertyRelative("_maxValue");
            // SerializedProperty variable = property.FindPropertyRelative("Variable");

            // Calculate rect for configuration button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += _popupStyle.margin.top;
            buttonRect.width = _popupStyle.fixedWidth + _popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int result = EditorGUI.Popup(buttonRect, valueType.enumValueIndex == (int)FloatValueType.Constant ? 0 : 1,
                _popupOptions, _popupStyle);

            valueType.enumValueIndex = result;

            var constantValue = valueType.enumValueIndex == (int)FloatValueType.Constant;
            if (constantValue)
                EditorGUI.PropertyField(position, minValue, GUIContent.none);
            else
            {
                const int spacing = 10;
                var minRect = new Rect(position);
                var maxRect = new Rect(position);
                minRect.width = maxRect.width = (position.width / 2) - spacing;
                maxRect.xMin = minRect.xMax + spacing;
                maxRect.xMax = position.xMax;
                EditorGUI.PropertyField(minRect, minValue, GUIContent.none);
                EditorGUI.PropertyField(maxRect, maxValue, GUIContent.none);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (!constantValue && minValue.floatValue > maxValue.floatValue)
                {
                    Debug.Log("Projectile min. spawn time cannot be greater than max spawn time. Setting max spawn to min.");
                    maxValue.floatValue = minValue.floatValue;
                }
                
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            const string inputClass = "c-float-value__input";
            var maxValueFieldName = $"{property.name}_maxVal";
            var container = new VisualElement();
            container.AddToClassList("c-float-value");
            var typeSelector = new EnumField();
            typeSelector.bindingPath = nameof(FloatValue.Type);
            typeSelector.Init(FloatValueType.Constant);
            typeSelector.style.width = 80;
            typeSelector.RegisterValueChangedCallback(evt =>
            {
                var maxValueField = container.Q<FloatField>(maxValueFieldName);
                var shouldShowMaxValueField = (FloatValueType)evt.newValue == FloatValueType.RandomBetweenTwoConstants;
                maxValueField.style.display = shouldShowMaxValueField ? DisplayStyle.Flex : DisplayStyle.None;
                ValidateTimingValues(container.Q<HelpBox>(HELP_BOX_NAME), property);
            });
            container.Add(typeSelector);

            var valueField = new FloatField();
            valueField.AddToClassList(inputClass);
            valueField.bindingPath = "_minValue";
            valueField.RegisterValueChangedCallback(_ => ValidateTimingValues(container.Q<HelpBox>(HELP_BOX_NAME), property));
            container.Add(valueField);

            var maxValueField = new FloatField();
            maxValueField.AddToClassList(inputClass);
            maxValueField.name = maxValueFieldName;
            maxValueField.bindingPath = "_maxValue";
            maxValueField.RegisterValueChangedCallback(_ => ValidateTimingValues(container.Q<HelpBox>(HELP_BOX_NAME), property));
            container.Add(maxValueField);
            var shouldShowMaxValue = property.FindPropertyRelative(nameof(FloatValue.Type)).enumValueIndex ==
                                     (int)FloatValueType.RandomBetweenTwoConstants;
            maxValueField.SetVisible(shouldShowMaxValue);

            var helpbox = CommonUI.CreateHelpBox("Min value cannot be more than max value", HELP_BOX_NAME,
                HelpBoxMessageType.Error);
            helpbox.SetVisible(false);
            container.Add(helpbox);
            return container;
        }

        private void ValidateTimingValues(HelpBox helpBox, SerializedProperty property)
        {
            var shouldShowMaxValue = property.FindPropertyRelative(nameof(FloatValue.Type)).enumValueIndex ==
                                     (int)FloatValueType.RandomBetweenTwoConstants;
            if (!shouldShowMaxValue)
            {
                helpBox.SetVisible(false);
                return;
            }
            var minVal = property.FindPropertyRelative("_minValue");
            var maxVal = property.FindPropertyRelative("_maxValue");
            helpBox.SetVisible(maxVal.floatValue < minVal.floatValue);
        }
    }
}