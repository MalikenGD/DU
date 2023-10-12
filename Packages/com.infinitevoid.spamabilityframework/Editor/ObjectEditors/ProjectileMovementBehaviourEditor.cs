using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(ProjectileMovementBehaviourSO))]
    [CanEditMultipleObjects]
    public class ProjectileMovementBehaviourEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement inspector = new VisualElement();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            inspector.styleSheets.Add(styleSheet);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            inspector.styleSheets.Add(styleSheet);


            var projectileMovementBehaviour = (ProjectileMovementBehaviourSO)target;
            if (projectileMovementBehaviour == null) return inspector;

            var serializedMovementObject = new SerializedObject(projectileMovementBehaviour);
            inspector.Add(CreateHeader2("Base settings"));
            AddPropToElement("_baseSpeed", serializedMovementObject, inspector);
            AddPropToElement("_desiredRotation", serializedMovementObject, inspector, tooltip: "");

            var straightMovementSettingsBox = new VisualElement();
            straightMovementSettingsBox.Add(CreateHeader2("Straight movement settings", "u-margin-top"));
            AddPropToElement("_3dMovement", serializedMovementObject, straightMovementSettingsBox);

            var curvedMovementSettingsBox = new VisualElement();
            curvedMovementSettingsBox.Add(CreateHeader2("Curved movement settings", "u-margin-top"));
            curvedMovementSettingsBox.Add(CommonUI.CreateHelpBox(
                "Curved movement can only be used for targeted projectiles. If you dont want to offset the projectile's start- or end-position, make sure that the Horizontal and Vertical curves start and end at 0. If strength is set to 0, the curve wont affect the projectile.",
                string.Empty));

            var horizCurveBox = CreateCurveBox(serializedObject.FindProperty("_horizontalMovement"),
                "Horizontal movement");
            horizCurveBox.name = "horizontal-movement";
            curvedMovementSettingsBox.Add(horizCurveBox);
            curvedMovementSettingsBox.Add(CreateCurveBox(serializedObject.FindProperty("_verticalMovement"),
                "Vertical movement"));
            curvedMovementSettingsBox.Add(CreateCurveBox(serializedObject.FindProperty("_speedCurve"),
                "Speed"));


            AddPropToElement("_movementType", serializedMovementObject, inspector, evt =>
            {
                var straightSelected = projectileMovementBehaviour.MovementType == ProjectileMovementType.Straight;
                var curved3dSelected = projectileMovementBehaviour.MovementType == ProjectileMovementType.AnimationCurve;
                var curved2dSelected = projectileMovementBehaviour.MovementType == ProjectileMovementType.AnimationCurve2d;
                straightMovementSettingsBox.SetVisible(straightSelected);
                curvedMovementSettingsBox.SetVisible(curved3dSelected || curved2dSelected);
                horizCurveBox.SetVisible(curved3dSelected);
                inspector.MarkDirtyRepaint();
            });

            inspector.Add(straightMovementSettingsBox);
            inspector.Add(curvedMovementSettingsBox);
            return inspector;
        }

        private VisualElement CreateCurveBox(SerializedProperty movementSettingsProp, string headerText)
        {
            var element = new VisualElement();
            element.AddToClassList("c-curve-box");
            element.AddToClassList("u-margin-bottom");
            var header = new VisualElement();
            header.AddToClassList("c-curve-box__header");
            header.Add(CreateHeader3(headerText));
            var activeIndicator = CreateLightIcon(LightIconColor.Green, "active-indicator",
                "This curve will affect the projectile");
            header.Add(activeIndicator);
            element.Add(header);

            var curveProp = movementSettingsProp.FindPropertyRelative("_curve");
            var strengthProp = movementSettingsProp.FindPropertyRelative("_strength");

            element.Add(new Label("Curve"));
            var curveField = new CurveField();
            curveField.BindProperty(curveProp);
            curveField.RegisterValueChangedCallback(evt =>
                SetActiveIndicatorVisibility(activeIndicator, strengthProp, curveProp));
            element.Add(curveField);

            var strengthLabel = new Label("Strength");
            strengthLabel.tooltip = "How much the curve should affect movement";
            strengthLabel.AddToClassList("u-margin-top--half");
            element.Add(strengthLabel);
            var strengthField = new PropertyField();
            strengthField.BindProperty(strengthProp);
            strengthField.RegisterValueChangeCallback(evt =>
                SetActiveIndicatorVisibility(activeIndicator, strengthProp, curveProp));
            element.Add(strengthField);

            CreateButton(element, "Clear", () =>
            {
                curveProp.animationCurveValue = new AnimationCurve();
                // curveProp.serializedObject.ApplyModifiedProperties();
                strengthProp.FindPropertyRelative("_minValue").floatValue = 0;
                strengthProp.FindPropertyRelative("_maxValue").floatValue = 0;
                strengthProp.FindPropertyRelative("Type").enumValueIndex = 0;
                // strengthProp.serializedObject.ApplyModifiedProperties();
                movementSettingsProp.serializedObject.ApplyModifiedProperties();
            }, additionalClass: "u-margin-top--half");
            return element;
        }

        private void SetActiveIndicatorVisibility(VisualElement activeIndicator, SerializedProperty strengthProp,
            SerializedProperty curveProp)
        {
            var strenghtHasValue = 0 < strengthProp.FindPropertyRelative("_minValue").floatValue;
            var curveHasValue = 0 < curveProp.animationCurveValue.length;
            activeIndicator.SetVisible(strenghtHasValue && curveHasValue);
        }
    }
}