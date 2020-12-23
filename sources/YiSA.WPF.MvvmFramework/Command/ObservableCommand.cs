using System;
using System.Reactive.Subjects;
using System.Threading;
using YiSA.Foundation.Common;
using YiSA.Foundation.Common.Extensions;
using YiSA.Foundation.Internal;

namespace YiSA.WPF.Command
{
    public class ObservableCommand<T> : DisposableHolder,ICommand<T> , IObservable<T>
    {
        private bool _canExecute;
        
        protected readonly Subject<T> _subject = new Subject<T>();
 
        public bool CanExecute(object parameter) => _canExecute;
        public bool CanExecute(T param) => _canExecute;

        public void Execute(T parameter) =>_subject.OnNext(parameter);

        public void Execute(object parameter)    => Execute((T)parameter);

        public event EventHandler? CanExecuteChanged;

        public ObservableCommand(IObservable<bool> trigger ,
            bool initializeCanExecute = true,
            Action<T>? action = null,
            SynchronizationContext? synchronizationContext = null)
        {
            _canExecute = initializeCanExecute;
            synchronizationContext ??= InternalSynchronizeContextUtility.GetOrCreate();
            
            trigger.Subscribe(value =>
            {
                _canExecute = value;
                synchronizationContext.Send(() =>
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
            }).DisposeBy(Disposables);

            if (action is {})
                _subject.Subscribe(action).DisposeBy(Disposables); 
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }
    }

    public class ObservableCommand : ObservableCommand<object>
    {
        public ObservableCommand(IObservable<bool> trigger, bool initializeCanExecute = true, Action? action = null, SynchronizationContext? synchronizationContext = null) 
            : base(trigger, initializeCanExecute, 
                action is null ? null : new Action<object>(_=>action()), 
                synchronizationContext) { }

        public void Execute() => _subject.OnNext(null!);
    }
}