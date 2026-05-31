using Autoclicker.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Autoclicker.UserControl
{
    public partial class ClickRepeat
    {
        public ClickRepeat()
        {
            InitializeComponent();
            // Sync initial state to match XAML defaults
            StoredMouseOptions.InfClick = false;
            StoredMouseOptions.RepeatCount = 1; // matches Text="1" in XAML
        }

        private void rbRepeatLimitHandler_Click(object sender, RoutedEventArgs e)
        {
            StoredMouseOptions.InfClick = rbRepeatUntilStopped.IsChecked == true;
        }

        private void tbRepeatTimes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(tbRepeatTimes.Text, out var repeatCount))
            {
                StoredMouseOptions.RepeatCount = Math.Max(1, repeatCount);
            }
        }
    }
}
