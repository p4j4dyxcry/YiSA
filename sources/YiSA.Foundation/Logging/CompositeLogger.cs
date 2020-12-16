using System;
using System.Collections.Generic;
using System.Linq;

namespace YiSA.Foundation.Logging
{
    public class CompositeLogger : ILogger , IDisposable
    {
        protected readonly List<ILogger> _loggers = new List<ILogger>();

        public CompositeLogger()
        {
            
        }
        
        public CompositeLogger(IEnumerable<ILogger> loggers)
        {
            _loggers.AddRange(loggers);
        }

        public CompositeLogger Add(ILogger logger)
        {
            _loggers.Add(logger);
            return this;
        }

        public void WriteLine(string message, LogLevel logLevel)
        {
            _loggers.ForEach(x=>x.WriteLine(message,logLevel));
        }

        public void Dispose()
        {
            foreach (var disposable in _loggers.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}