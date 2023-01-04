using System;
using System.Threading;
using System.Threading.Tasks;
using YiSA.Foundation.Logging;

namespace YiSA.Foundation.Common
{
    public class AsyncSetter<T>
    {
        private readonly ILogger? _errorLogger;
        private readonly SynchronizationContext? _synchronizationContext;
        public event EventHandler? Completed;
        
        public AsyncSetter(ILogger? errorLogger , SynchronizationContext? synchronizationContext = null)
        {
            _errorLogger = errorLogger;
            _synchronizationContext = synchronizationContext;
        }

        public Task SetValueAsync(Action<T> setter , Func<T> factory )
        {
            T? value = default;
            return Task.Run(() => value = factory())
            .ContinueWith((x) =>
            {
                if (_errorLogger is {} && x.Exception  is {})
                {
                    _errorLogger?.WriteLine(x.Exception.ToString(),LogLevel.Error);
                }
                else
                {
                    if (_synchronizationContext != null)
                    {
                        _synchronizationContext.Post(()=>
                        {
                            setter(value!);
                            Completed?.Invoke(this,EventArgs.Empty);
                        });
                    }
                    else
                    {
                        setter(value!);
                        Completed?.Invoke(this,EventArgs.Empty);
                    }
                }
            });
        }
    }
}
