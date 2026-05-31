namespace Autoclicker.Model
{
    public class StoredMouseOptions
    {
        public static string MouseMode { get; set; } = "Left";
        public static bool DoubleClick { get; set; } = false;
        public static int ClickSpeed { get; set; } = 20;
        public static int ClickDelayHours { get; set; } = 0;
        public static int ClickDelayMinutes { get; set; } = 0;
        public static int ClickDelaySeconds { get; set; } = 0;
        public static int ClickDelayMilliseconds { get; set; } = 20;
        public static bool InfClick { get; set; } = false;
        public static int RepeatCount { get; set; } = 1;
        public static int RepeatPerSecond { get; set; } = 20;
        public static System.Windows.Input.Key StartStopKey { get; set; } = System.Windows.Input.Key.F6;
        public static bool AlwaysOnTop { get; set; } = true;

        public static int ClickDelayTotalMilliseconds =>
            (((ClickDelayHours * 60) + ClickDelayMinutes) * 60 + ClickDelaySeconds) * 1000 + ClickDelayMilliseconds;
    }
}
