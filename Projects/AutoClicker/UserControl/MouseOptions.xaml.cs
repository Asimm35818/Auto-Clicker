using System;
using System.Linq;
using System.Windows.Controls;
using Autoclicker.Model;

namespace Autoclicker.UserControl
{
    public partial class MouseOptions
    {
        private readonly string[] MouseButtons = { "Left", "Middle", "Right" };
        private readonly string[] ClickTypes = { "Single", "Double" };

        public MouseOptions()
        {
            InitializeComponent();
            cbMouseButtons.ItemsSource = MouseButtons.ToArray();
            cbClickTypes.ItemsSource = ClickTypes.ToArray();
            cbMouseButtons.Text = string.IsNullOrWhiteSpace(StoredMouseOptions.MouseMode) ? "Left" : StoredMouseOptions.MouseMode;
            cbMouseButtons.SelectedIndex = Array.IndexOf(MouseButtons, cbMouseButtons.Text);
            cbClickTypes.Text = StoredMouseOptions.DoubleClick ? "Double" : "Single";
            cbClickTypes.SelectedIndex = StoredMouseOptions.DoubleClick ? 1 : 0;
        }

        private void cbMouseButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMouseButtons.SelectedItem is string mode)
            {
                StoredMouseOptions.MouseMode = mode;
            }
        }

        private void cbClickTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbClickTypes.SelectedItem is string clickType)
            {
                StoredMouseOptions.DoubleClick = clickType == "Double";
            }
        }
    }
}
