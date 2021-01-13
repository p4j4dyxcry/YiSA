using System;
using System.Collections.Generic;

namespace YiSA.WPF.Common
{
    public class DisposableBindable : Bindable , IDisposable
    {
        public IList<IDisposable> Disposables => _disposableHolder.Disposables;
        private DisposableHolder _disposableHolder = new DisposableHolder();
        
        public void Dispose()
        {
            _disposableHolder.Dispose();
        }
    }
}