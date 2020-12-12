using System;
using System.Collections.Generic;
using YiSA.Foundation.Common.Extensions;

namespace YiSA.WPF.Common
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
                Disposables.ForEach(item => item.Dispose());
            }
            _disposed = true;
        }
    }
}