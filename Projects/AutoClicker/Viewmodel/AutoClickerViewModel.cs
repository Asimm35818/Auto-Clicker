using Autoclicker.Model;
using Autoclicker.MVVM;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Autoclicker.Viewmodel
{
    public class AutoClickerViewModel : ViewModelBase
    {
        private readonly AutoClickerService _autoClickerService = new();

        public ObservableCollection<StoredMouseOptions>? MouseOptions { get; set; }

        private StoredMouseOptions? mouseMode;

        public StoredMouseOptions? MouseMode
        {
            get => mouseMode;
            set
            {
                mouseMode = value;
                OnPropertyChanged();
            }
        }

        public string StartButtonText => $"Start ({StoredMouseOptions.StartStopKey})";

        public bool IsRunning => _autoClickerService.IsRunning;

        public AutoClickerViewModel()
        {
            _autoClickerService.Stopped += () =>
            {
                // The service fires Stopped on a thread-pool thread; marshal to UI.
                Application.Current?.Dispatcher.Invoke(NotifyRunningState);
            };
        }

        public void NotifyHotkeyChanged()
        {
            OnPropertyChanged(nameof(StartButtonText));
        }

        private void NotifyRunningState()
        {
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(StartEnabled));
            OnPropertyChanged(nameof(StopEnabled));
        }

        public Task ToggleAutoClickerAsync()
        {
            var task = _autoClickerService.ToggleAsync();
            NotifyRunningState();
            return task;
        }

        public Task StopAutoClickerAsync()
        {
            var task = _autoClickerService.StopAsync();
            NotifyRunningState();
            return task;
        }

        public bool StartEnabled => !IsRunning;

        public bool StopEnabled => IsRunning;
    }
}
