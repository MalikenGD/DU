using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.Common.Elements
{
    /// <summary>
    /// A label with a icon that can be toggled visible/invisible
    /// </summary>
    public class IconLabel : VisualElement
    {
        private VisualElement _icon;
        public IconLabel(string text, IconType iconType, string iconTooltip, string additionalClass = null)
        {
            if(!string.IsNullOrWhiteSpace(additionalClass))
                this.AddToClassList(additionalClass);
            
            _icon = new Label();
            _icon.name = "icon";
            _icon.AddToClassList(GetIconClass(iconType));
            _icon.SetVisible(false);
            _icon.tooltip = iconTooltip;
            this.Add(_icon);
            
            this.Add(new Label(text));
        }

        public void SetIconVisible(bool isVisible) => _icon.SetVisible(isVisible);

        private string GetIconClass(IconType iconType) =>
            iconType switch
            {
                IconType.Warning => "icon-warning",
                IconType.Error => "icon-error",
                _ => string.Empty
            };


        public enum IconType
        {
            Warning,
            Error
        }
    }
}