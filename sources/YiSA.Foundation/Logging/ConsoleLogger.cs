using System;

namespace YiSA.Foundation.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string message, LogLevel logLevel)
        {
            var prev = Console.ForegroundColor;
            var color = logLevel switch
            {
                LogLevel.Error   => ConsoleColor.DarkRed,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Success => ConsoleColor.DarkGreen,
                LogLevel.Debug => ConsoleColor.Gray,

                _ => ConsoleColor.DarkGray,
            };
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }
    }
}