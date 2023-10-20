using System;
using InfiniteVoid.SpamFramework.Editor.Common;
using InfiniteVoid.SpamFramework.Editor.Common.Elements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.PoolingWindow
{
    public class PooledVfxRowItem : VisualElement
    {
        public PooledVfxRowItem(string name,
            SerializedProperty countProperty,
            SerializedObject serializedSettings,
            string usedIn,
            bool showRemoveButton,
            Action onRemoveClick)
        {
            this.AddToClassList("c-pooled-item--row");
            
            var iconLabel = new IconLabel(name,
                IconLabel.IconType.Error,
                "Pooled count must be at least 1",
                additionalClass: "c-pooled-item__name");
            this.Add(iconLabel);

            // var nameElement = new Label(name);
            // nameElement.AddToClassList("c-pooled-item__name");
            // this.Add(nameElement);
            
            var countPropField = new PropertyField(countProperty);
            countPropField.label = string.Empty;
            countPropField.AddToClassList("c-pooled-item__count");
            countPropField.BindProperty(serializedSettings);
            this.Add(countPropField);
            countPropField.RegisterValueChangeCallback(ev => iconLabel.SetIconVisible(ev.changedProperty.intValue == 0));
            
            var usedInLabel = new Label(usedIn);
            usedInLabel.tooltip = usedIn;
            usedInLabel.AddToClassList("c-pooled-item__meta");
            this.Add(usedInLabel);
            
            if (showRemoveButton)
            {
                var removeButton = CommonUI.CreateButton(this, "Remove pooled item", onRemoveClick, additionalClass: "c-button--red");
                removeButton.tooltip = "This object is not referenced by any ability and can be removed.";
            }
        }
    }
}