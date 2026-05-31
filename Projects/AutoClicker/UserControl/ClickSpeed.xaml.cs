using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Autoclicker.Model;

namespace Autoclicker.UserControl
{
    public partial class ClickSpeed
    {
        private bool _isReady;

        public ClickSpeed()
        {
            InitializeComponent();
            Loaded += (_, _) => SyncFromStoredOptions();
        }

        private void SyncFromStoredOptions()
        {
            _isReady = false;
            tbHours.Text = StoredMouseOptions.ClickDelayHours.ToString();
            tbMinutes.Text = StoredMouseOptions.ClickDelayMinutes.ToString();
            tbSeconds.Text = StoredMouseOptions.ClickDelaySeconds.ToString();
            tbMilliseconds.Text = StoredMouseOptions.ClickDelayMilliseconds.ToString();
            _isReady = true;
        }

        private void ClickSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isReady)
                return;

            if (tbHours != null)
                StoredMouseOptions.ClickDelayHours = ParseOrZero(tbHours.Text);

            if (tbMinutes != null)
                StoredMouseOptions.ClickDelayMinutes = ParseOrZero(tbMinutes.Text);

            if (tbSeconds != null)
                StoredMouseOptions.ClickDelaySeconds = ParseOrZero(tbSeconds.Text);

            if (tbMilliseconds != null)
                StoredMouseOptions.ClickDelayMilliseconds = ParseOrZero(tbMilliseconds.Text);
        }

        private void ClickSpeed_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not TextBox tb || !_isReady)
                return;

            if (string.IsNullOrWhiteSpace(tb.Text))
                tb.Text = "0";
        }

        private static int ParseOrZero(string text) =>
            int.TryParse(text, out var v) ? Math.Max(0, v) : 0;
    }
}
