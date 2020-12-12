using System;

namespace YiSA.Foundation.Logging
{
    public class DelegateLogger : ILogger
    {
        private readonly Action<string, LogLevel> _action;

        public DelegateLogger(Action<string, LogLevel> action)
        {
            _action = action;
        }
        public DelegateLogger(Action<string> action)
        {
            _action = (m,l)=>action(m);
        }

        public void WriteLine(string message, LogLevel logLevel)
        {
            _action(message, logLevel);
        }
    }
}