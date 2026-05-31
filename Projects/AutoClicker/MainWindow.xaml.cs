using Autoclicker.Model;
using Autoclicker.View;
using Autoclicker.Viewmodel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Autoclicker
{
    public partial class MainWindow : Window
    {
        private readonly AutoClickerViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new AutoClickerViewModel();
            DataContext = _viewModel;
            Topmost = StoredMouseOptions.AlwaysOnTop;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Height = 468;
            Activate();
            Focus();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 || IsOverInteractiveControl(e.OriginalSource as DependencyObject))
                return;

            TryDragMove();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 || IsOverInteractiveControl(e.OriginalSource as DependencyObject))
                return;

            TryDragMove();
        }

        private void TryDragMove()
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;

            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
                // DragMove can throw if the mouse button was released before
                // the call was reached — safe to ignore.
            }
        }

        /// <summary>
        /// Walks up the visual tree from <paramref name="source"/> and returns true
        /// if any ancestor (or the element itself) is an interactive control that
        /// should receive clicks rather than letting the window drag.
        /// </summary>
        private static bool IsOverInteractiveControl(DependencyObject? source)
        {
            var current = source;
            while (current is not null && current is not Window)
            {
                if (current is System.Windows.Controls.Button
                    or System.Windows.Controls.Primitives.ToggleButton
                    or System.Windows.Controls.TextBox
                    or System.Windows.Controls.ComboBox
                    or System.Windows.Controls.RadioButton
                    or System.Windows.Controls.CheckBox
                    or System.Windows.Controls.Slider
                    or System.Windows.Controls.Primitives.ScrollBar)
                {
                    return true;
                }

                current = VisualTreeHelper.GetParent(current)
                          ?? LogicalTreeHelper.GetParent(current);
            }

            return false;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public System.Threading.Tasks.Task ToggleAutoClickerAsync() => _viewModel.ToggleAutoClickerAsync();

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.ToggleAutoClickerAsync();
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.StopAutoClickerAsync();
        }

        private void btnHotkeySettings_Click(object sender, RoutedEventArgs e)
        {
            var popup = new HotkeyCapture(_viewModel)
            {
                Owner = this
            };

            btnHotkeySettings.IsEnabled = false;
            popup.Closed += (_, _) => btnHotkeySettings.IsEnabled = true;
            popup.ShowDialog();
        }

        private void tgAlwaysOnTop_Checked(object sender, RoutedEventArgs e)
        {
            StoredMouseOptions.AlwaysOnTop = true;
            Topmost = true;
        }

        private void tgAlwaysOnTop_Unchecked(object sender, RoutedEventArgs e)
        {
            StoredMouseOptions.AlwaysOnTop = false;
            Topmost = false;
        }

        private void btnCodeLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://asim-m.dev/",
                UseShellExecute = true
            });
        }
    }
}
