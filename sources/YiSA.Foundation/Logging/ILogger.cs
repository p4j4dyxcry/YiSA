using System;
using System.Text;

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

    public static class LoggerExtensions
    {
        public static void WriteLine(this ILogger logger, string message) => logger.WriteLine(message, LogLevel.Default);
        public static void Error(this ILogger logger, string message) => logger.WriteLine(message, LogLevel.Error);
        public static void Error(this ILogger logger, Exception e, string? message = null) => logger.Error(_exceptionToMessage(message,e));
        public static void Warning(this ILogger logger, string message) => logger.WriteLine(message, LogLevel.Warning);
        public static void Warning(this ILogger logger, Exception e, string? message = null) => logger.Warning(_exceptionToMessage(message,e));
        private static string _exceptionToMessage(string? header, Exception e)
        {
            var builder = new StringBuilder();

            if (header is { })
                builder.AppendLine(header);
            builder.Append(e);
            return builder.ToString();
        }
    }
    
}