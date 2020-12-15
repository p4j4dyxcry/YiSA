using System;
using System.Collections.Generic;

namespace YiSA.Markup.Common
{
    public class DisposableBindable : Bindable , IDisposable
    {
        public IList<IDisposable> Disposables { get; } = new List<IDisposable>();
        private bool _disposed = false;
        
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                foreach (var disposable in Disposables)
                {
                    disposable.Dispose();
                }
            }
            _disposed = true;
        }
    }
}