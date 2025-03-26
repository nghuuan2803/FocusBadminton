public class Logger
{
    private static readonly Lazy<Logger> _lazyInstance = new Lazy<Logger>(() => new Logger());
    private readonly string _logFilePath;
    private static readonly object _lock;

    private Logger()
    {
        _logFilePath = "logs/app_log.txt";
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
    }
    public static Logger Instance
    {
        get
        {
            return _lazyInstance.Value;
        }
    }
    public void Log(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        lock (_lock)
        {
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
    }
}