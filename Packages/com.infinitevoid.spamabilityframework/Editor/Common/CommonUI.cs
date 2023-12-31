﻿using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.Common
{
    public static class CommonUI
    {
        public static Label CreateHeader(string text, string name = null)
        {
            var header = new Label(text);
            header.AddToClassList("header");
            if (!string.IsNullOrWhiteSpace(name))
                header.name = name;
            return header;
        }

        public static Label CreateHeader2(string text, string additionalClass = null)
        {
            var header = new Label(text);
            header.AddToClassList("header-2");
            if (!string.IsNullOrWhiteSpace(additionalClass))
                header.AddToClassList(additionalClass);
            return header;
        }

        public static VisualElement CreateHeader3(string text, string additionalClass = null)
        {
            var header = new Label(text);
            header.AddToClassList("header-3");
            if (!string.IsNullOrWhiteSpace(additionalClass))
                header.AddToClassList(additionalClass);
            return header;
        }

        public static HelpBox CreateHelpBox(string text, string name,
            HelpBoxMessageType messageType = HelpBoxMessageType.Info, string additionalClass = null)
        {
            var helpBox = new HelpBox(text, messageType);
            helpBox.name = name;
            if (!string.IsNullOrWhiteSpace(additionalClass))
                helpBox.AddToClassList(additionalClass);
            // csharpHelpBox.AddToClassList("some-styled-help-box");
            return helpBox;
        }

        /// <summary>
        /// Creates a box with a header and context area in the given parent. Returns the context box.
        /// </summary>
        /// <param name="headerText"></param>
        /// <param name="parent"></param>
        /// <param name="contentAsRow"></param>
        /// <param name="additionalClass"></param>
        /// <param name="canBeHidden"></param>
        /// <returns></returns>
        public static Box CreateBox(string headerText, VisualElement parent, bool contentAsRow = false,
            string additionalClass = null, bool canBeHidden = false, string helpUrl = null)
        {
            var box = new Box();
            box.AddToClassList("c-box");
            if (!string.IsNullOrWhiteSpace(additionalClass))
                box.AddToClassList(additionalClass);
            var headerBox = new Box();
            headerBox.AddToClassList("c-box__header");

            box.Add(headerBox);
            var header = new Label(headerText);
            headerBox.Add(header);

            if (!string.IsNullOrWhiteSpace(helpUrl))
            {
                var helpLabel = new Label();
                helpLabel.AddToClassList("icon-help");
                headerBox.Add(helpLabel);
                helpLabel.RegisterCallback<ClickEvent>(_ => Application.OpenURL(helpUrl));
                helpLabel.tooltip = $"Open online documentation at {helpUrl}";
            }
            
            var contentBox = new Box();
            contentBox.AddToClassList("c-box__content");
            if (contentAsRow)
                contentBox.AddToClassList("c-box__content--row");
            box.Add(contentBox);
            // headerBox.Add(CommonUI.CreateHeader2(header));
            parent.Add(box);
            
            if(canBeHidden)
                headerBox.RegisterCallback<PointerDownEvent>(e =>
                {
                    e.StopImmediatePropagation();
                    contentBox.ToggleInClassList("hidden");
                });
            return contentBox;
        }

        public static void AddPropToElement(string propertyName, SerializedObject boundObject, VisualElement parentBox,
            EventCallback<SerializedPropertyChangeEvent> onChange = null, string additionalClass = null,
            string tooltip = null, string controlName = null)
        {
            var prop = boundObject.FindProperty(propertyName);
            var propField = new PropertyField(prop);
            if (!string.IsNullOrWhiteSpace(controlName))
                propField.name = controlName;
            if (!string.IsNullOrWhiteSpace(tooltip))
                propField.tooltip = tooltip;
            if (onChange != null)
                propField.RegisterValueChangeCallback(onChange);
            propField.Bind(boundObject);
            if (!string.IsNullOrWhiteSpace(additionalClass))
                propField.AddToClassList(additionalClass);
            parentBox.Add(propField);
        }

        public static void CreateEmptyReferenceBox(Box parentBox, string buttonText,
            Action onNewReferenceClick)
        {
            var button = new Button();
            button.text = buttonText;
            button.clicked += onNewReferenceClick;
            parentBox.Add(button);
        }

        public static Button CreateButton(VisualElement parent, string buttonText, Action onClick, string name = null, string additionalClass = null)
        {
            var button = new Button();
            button.text = buttonText;
            button.clicked += onClick;
            parent.Add(button);
            if (!string.IsNullOrWhiteSpace(name))
                button.name = name;
            if(!string.IsNullOrWhiteSpace(additionalClass))
                button.AddToClassList(additionalClass);
            return button;
        }
        
        public static Button CreateCTAButton(VisualElement parent, string buttonText, Action onClick, string name = null)
        {
            var button = CreateButton(parent, buttonText, onClick, name);
            button.AddToClassList("c-button--blue");
            return button;
        }

        public static VisualElement CreateLightIcon(LightIconColor color, string name = null, string tooltip = null)
        {
            var lightElement = new VisualElement();
            lightElement.name = name;
            lightElement.AddToClassList("icon-light");
            string iconClass;
            if (color == LightIconColor.Green)
                iconClass = "icon-light--green";
            else if (color == LightIconColor.Orange)
                iconClass = "icon-light--orange";
            else
                iconClass = "icon-light--red";
            lightElement.AddToClassList(iconClass);
            lightElement.tooltip = tooltip;
            return lightElement;
        }

        public enum LightIconColor
        {
            Green,
            Orange,
            Red
        }
    }
}