public class Logger
{
    private static readonly Lazy<Logger> _lazyInstance = new Lazy<Logger>(() => new Logger());
    private readonly string _logFilePath;

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
        lock (this)
        {
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
    }
}