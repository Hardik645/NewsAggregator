namespace NewsAggregator.API.Utils
{
    public static class ExceptionLogger
    {
        private static string? _logDirectory;

        public static void Init(IConfiguration configuration)
        {
            _logDirectory = configuration["Logging:LogDirectory"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        public static void LogException(Exception exception)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_logDirectory))
                    throw new InvalidOperationException("ExceptionLogger is not initialized. Call ExceptionLogger.Init() at startup.");

                if (!Directory.Exists(_logDirectory))
                    Directory.CreateDirectory(_logDirectory);

                string logFile = Path.Combine(_logDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.log");
                string logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {exception.GetType().FullName}: {exception.Message}{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}";

                File.AppendAllText(logFile, logEntry);
            }
            catch (Exception ex)
            {
                string logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
                Console.WriteLine("Failed to log exception: " + logEntry);
            }
        }
    }
}