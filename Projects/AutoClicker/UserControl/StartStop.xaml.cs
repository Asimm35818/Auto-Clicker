using Autoclicker.Viewmodel;
using Autoclicker.View;
using Autoclicker.Model;
using System.Windows;

namespace Autoclicker.UserControl
{
    /// <summary>
    /// Interaction logic for StartStop.xaml
    /// </summary>
    public partial class StartStop
    {
        private HotkeyCapture? _hotkeyCaptureWindow;

        public StartStop()
        {
            InitializeComponent();
        }

        private void StartStop_Loaded(object sender, RoutedEventArgs e)
        {
            tgAlwaysOnTop.IsChecked = StoredMouseOptions.AlwaysOnTop;
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is AutoClickerViewModel viewModel)
            {
                await viewModel.ToggleAutoClickerAsync();
            }

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is AutoClickerViewModel viewModel)
            {
                await viewModel.StopAutoClickerAsync();
            }

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void btnHotkeySettings_Click(object sender, RoutedEventArgs e)
        {
            if (_hotkeyCaptureWindow is not null)
            {
                return;
            }

            if (Window.GetWindow(this)?.DataContext is not AutoClickerViewModel viewModel)
            {
                return;
            }

            var owner = Window.GetWindow(this);
            _hotkeyCaptureWindow = new HotkeyCapture(viewModel)
            {
                Owner = owner
            };
            _hotkeyCaptureWindow.Closed += (_, _) =>
            {
                btnHotkeySettings.IsEnabled = true;
                _hotkeyCaptureWindow = null;
            };

            btnHotkeySettings.IsEnabled = false;
            _hotkeyCaptureWindow.ShowDialog();
        }

        private void tgAlwaysOnTop_Checked(object sender, RoutedEventArgs e)
        {
            StoredMouseOptions.AlwaysOnTop = true;
            if (Window.GetWindow(this) is Window owner)
            {
                owner.Topmost = true;
            }
        }

        private void tgAlwaysOnTop_Unchecked(object sender, RoutedEventArgs e)
        {
            StoredMouseOptions.AlwaysOnTop = false;
            if (Window.GetWindow(this) is Window owner)
            {
                owner.Topmost = false;
            }
        }
    }
}
