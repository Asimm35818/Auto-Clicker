using System.Windows;
using System.Windows.Input;
using Autoclicker.Model;
using Autoclicker.Viewmodel;

namespace Autoclicker.View
{
    public partial class HotkeyCapture : Window
    {
        private readonly AutoClickerViewModel _viewModel;

        public HotkeyCapture(AutoClickerViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            AddHandler(Keyboard.PreviewKeyDownEvent, new KeyEventHandler(Window_PreviewKeyDown), true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbCurrentKey.Text = StoredMouseOptions.StartStopKey.ToString();
            Activate();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                return;
            }

            StoredMouseOptions.StartStopKey = e.Key;
            _viewModel.NotifyHotkeyChanged();
            App.RefreshHotkeyRegistration();
            DialogResult = true;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;

            try { DragMove(); }
            catch (System.InvalidOperationException) { }
        }
    }
}
