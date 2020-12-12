using System;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Logging
{
    public class RelayLogger : CompositeLogger
    {
        public IDisposable Subscribe(Action<string,LogLevel> action)
        {
            var logger = new DelegateLogger(action);
            _loggers.Add(logger);
            return InternalDisposable.Make(() => _loggers.Remove(logger));
        }
    }
}