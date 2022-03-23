using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// An attached property helper to specify for which target component a AccessText is for
    /// </summary>
    public static class AccessTextHelper
    {
        public static UIElement GetTarget(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(TargetProperty);
        }

        public static void SetTarget(DependencyObject obj, UIElement value)
        {
            obj.SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached("Target", typeof(UIElement), typeof(AccessTextHelper), new PropertyMetadata(null, OnTargetChanged));

        private static void OnTargetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is AccessText accessText)
            {
                AccessKeyManager.AddAccessKeyPressedHandler(accessText, (object sender, AccessKeyPressedEventArgs e2) => e2.Target = e.NewValue as UIElement);
            }
        }
    }
}
