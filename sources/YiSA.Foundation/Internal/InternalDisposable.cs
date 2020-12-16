using System;
using System.Collections.Generic;

namespace YiSA.Foundation.Internal
{
    internal static class InternalDisposable
    {
        public static IDisposable Empty { get; } = new InternalEmptyDisposable();
        public static IDisposable Make(Action dispose) => new InternalDelegateDisposable(dispose);
    }

    internal class InternalDelegateDisposable : IDisposable
    {
        private readonly Action _disposeAction;

        public InternalDelegateDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }
        
        public void Dispose()
        {
            _disposeAction();
        }
    }

    internal class InternalEmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public class DisposableHolder : IDisposable
    {
        internal IList<IDisposable> Disposables { get; }= new List<IDisposable>();
        
        public void Dispose()
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
        }
    }
}