
namespace Commons
{
    public static class Logger
    {
        public const bool WriteLogToConsole = false;
        public const bool WriteLogToFile = false;

        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "application.log");
        private static readonly object _fileLock = new object();

        public static void Debug(string message) => Log("DEBUG", message);
        public static void Info(string message) => Log("INFO", message);
        public static void Warning(string message) => Log("WARN", message);
        public static void Error(string message) => Log("ERROR", message);

        private static void Log(string level, string message)
        {
            string timestamped = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";

            if (WriteLogToConsole)
            {
                Console.WriteLine(timestamped);
            }

            if (WriteLogToFile)
            {
                try
                {
                    lock (_fileLock)
                    {
                        File.AppendAllText(_logFilePath, timestamped + Environment.NewLine);
                    }
                }
                catch
                {
                    if (WriteLogToConsole)
                    {
                        Console.WriteLine($"[Logger Error] Unable to write to log file at {_logFilePath}");
                    }
                }
            }
        }
    }
}
