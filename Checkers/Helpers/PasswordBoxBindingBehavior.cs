using System.Windows;
using System.Windows.Controls;

namespace Checkers.Helpers
{
    public class PasswordBoxBindingBehavior
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxBindingBehavior), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static bool _updating;

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            passwordBox.PasswordChanged -= HandlePasswordChanged;

            if (!_updating)
                passwordBox.Password = (string)e.NewValue;

            passwordBox.PasswordChanged += HandlePasswordChanged;
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            _updating = true;
            SetBoundPassword(passwordBox, passwordBox.Password);
            _updating = false;
        }
    }
}
