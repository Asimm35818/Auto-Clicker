using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using Autoclicker.Model;

namespace Autoclicker
{
    public partial class App : Application
    {
        private const int HotkeyId = 0xBEEF;
        private const int WmHotkey = 0x0312;
        private const uint ModNone = 0x0000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private HwndSource? _source;
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mainWindow = new MainWindow();
            _mainWindow.SourceInitialized += MainWindow_SourceInitialized;
            _mainWindow.Closed += (_, _) => Shutdown();
            _mainWindow.Show();
        }

        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            if (_mainWindow is null)
            {
                return;
            }

            var handle = new WindowInteropHelper(_mainWindow).Handle;
            _source = HwndSource.FromHwnd(handle);
            _source?.AddHook(WndProc);
            RegisterCurrentHotkey(handle);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WmHotkey && wParam.ToInt32() == HotkeyId)
            {
                handled = true;
                _ = _mainWindow?.ToggleAutoClickerAsync();
            }

            return IntPtr.Zero;
        }

        public static void RefreshHotkeyRegistration()
        {
            var app = Current as App;
            app?.RefreshHotkeyRegistrationCore();
        }

        private void RefreshHotkeyRegistrationCore()
        {
            if (_mainWindow is null)
            {
                return;
            }

            var handle = new WindowInteropHelper(_mainWindow).Handle;
            UnregisterHotKey(handle, HotkeyId);
            RegisterCurrentHotkey(handle);
        }

        private void RegisterCurrentHotkey(IntPtr handle)
        {
            var key = StoredMouseOptions.StartStopKey;
            if (!RegisterHotKey(handle, HotkeyId, ModNone, (uint)KeyInterop.VirtualKeyFromKey(key)))
            {
                MessageBox.Show($"Failed to register global hotkey: {key}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mainWindow is not null)
            {
                var handle = new WindowInteropHelper(_mainWindow).Handle;
                UnregisterHotKey(handle, HotkeyId);
            }

            _source?.RemoveHook(WndProc);
            base.OnExit(e);
        }
    }
}
