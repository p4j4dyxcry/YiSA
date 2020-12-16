namespace YiSA.Foundation.Logging
{
    public enum LogLevel : uint
    {
        Fatal       = 2048,
        Error       = 1024,
        Warning     = 512,
        Success     = 256,
        Information = 128,
        Debug       = 64,
        Default     = 0,
    }
    
    public interface ILogger
    {
        void WriteLine(string message, LogLevel logLevel);
    }
}