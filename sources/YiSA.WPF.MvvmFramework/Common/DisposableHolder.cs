using System;
using System.Collections.Generic;

namespace YiSA.WPF.Common
{
    public class DisposableHolder : IDisposable
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