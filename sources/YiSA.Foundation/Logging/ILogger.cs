namespace YiSA.Foundation.Logging
{
    public enum LogLevel
    {
        Error,
        Warning,
        Success,
        Information,
        Debug,
        Default,
    }
    
    public interface ILogger
    {
        void WriteLine(string message, LogLevel logLevel);
    }
}