using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Autoclicker.Model
{
    internal sealed class AutoClickerService
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dX, uint dY, uint dwData, IntPtr dwExtraInfo);

        private const uint MouseEventLeftDown = 0x0002;
        private const uint MouseEventLeftUp = 0x0004;
        private const uint MouseEventRightDown = 0x0008;
        private const uint MouseEventRightUp = 0x0010;
        private const uint MouseEventMiddleDown = 0x0020;
        private const uint MouseEventMiddleUp = 0x0040;

        private readonly object _gate = new();
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _runningTask;

        /// <summary>
        /// Raised on the thread pool when the clicker stops (either cancelled or
        /// finished its repeat count). Subscribe to update UI state.
        /// </summary>
        public event Action? Stopped;

        public bool IsRunning
        {
            get
            {
                lock (_gate)
                {
                    return _runningTask is { IsCompleted: false } && _cancellationTokenSource is not null;
                }
            }
        }

        public Task ToggleAsync()
        {
            if (IsRunning)
            {
                return StopAsync();
            }

            StartAsync();
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            lock (_gate)
            {
                if (_runningTask is { IsCompleted: false })
                {
                    return Task.CompletedTask;
                }

                _cancellationTokenSource = new CancellationTokenSource();
                var cts = _cancellationTokenSource;
                _runningTask = RunAsync(cts.Token).ContinueWith(_ =>
                {
                    lock (_gate)
                    {
                        if (_cancellationTokenSource == cts)
                            _cancellationTokenSource = null;
                    }
                    Stopped?.Invoke();
                }, TaskScheduler.Default);
                return Task.CompletedTask;
            }
        }

        public async Task StopAsync()
        {
            Task? runningTask;
            CancellationTokenSource? cancellationTokenSource;

            lock (_gate)
            {
                runningTask = _runningTask;
                cancellationTokenSource = _cancellationTokenSource;
                _cancellationTokenSource = null;
            }

            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            if (runningTask is not null)
            {
                try
                {
                    await runningTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            lock (_gate)
            {
                if (_runningTask == runningTask)
                {
                    _runningTask = null;
                }
            }
        }

        private static async Task RunAsync(CancellationToken cancellationToken)
        {
            var remainingClicks = StoredMouseOptions.InfClick ? int.MaxValue : Math.Max(1, StoredMouseOptions.RepeatCount);
            var delayMs = GetDelayMilliseconds();
            var stopwatch = Stopwatch.StartNew();
            var nextTick = TimeSpan.Zero;

            while (remainingClicks > 0 && !cancellationToken.IsCancellationRequested)
            {
                nextTick += TimeSpan.FromMilliseconds(delayMs);
                await ClickOnceAsync(cancellationToken).ConfigureAwait(false);
                remainingClicks--;

                if (remainingClicks <= 0 || cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var remaining = nextTick - stopwatch.Elapsed;
                if (remaining > TimeSpan.Zero)
                {
                    await Task.Delay(remaining, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await Task.Yield();
                }
            }
        }

        private static int GetDelayMilliseconds()
        {
            var delayFromSpeed = StoredMouseOptions.ClickDelayTotalMilliseconds;
            if (delayFromSpeed > 0)
            {
                return delayFromSpeed;
            }

            var clicksPerSecond = Math.Max(1, StoredMouseOptions.RepeatPerSecond);
            return Math.Max(1, (int)Math.Round(1000d / clicksPerSecond));
        }

        private static async Task ClickOnceAsync(CancellationToken cancellationToken)
        {
            var isDoubleClick = StoredMouseOptions.DoubleClick || string.Equals(StoredMouseOptions.MouseMode, "Double", StringComparison.OrdinalIgnoreCase);
            var clickCount = isDoubleClick ? 2 : 1;

            for (var i = 0; i < clickCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                PerformMouseClick();

                if (i == 0 && isDoubleClick)
                {
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static void PerformMouseClick()
        {
            switch (StoredMouseOptions.MouseMode)
            {
                case "Middle":
                    mouse_event(MouseEventMiddleDown, 0, 0, 0, IntPtr.Zero);
                    mouse_event(MouseEventMiddleUp, 0, 0, 0, IntPtr.Zero);
                    break;
                case "Right":
                    mouse_event(MouseEventRightDown, 0, 0, 0, IntPtr.Zero);
                    mouse_event(MouseEventRightUp, 0, 0, 0, IntPtr.Zero);
                    break;
                default:
                    mouse_event(MouseEventLeftDown, 0, 0, 0, IntPtr.Zero);
                    mouse_event(MouseEventLeftUp, 0, 0, 0, IntPtr.Zero);
                    break;
            }
        }
    }
}
