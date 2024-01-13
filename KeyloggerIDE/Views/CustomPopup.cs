using Avalonia;
using Avalonia.Controls.Primitives;

namespace KeyloggerIDE.Views
{
    public class CustomPopup : PickerPresenterBase
    {
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<CustomPopup, string>(nameof(Text));
    }
}
