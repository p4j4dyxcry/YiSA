using System;
using System.Windows.Input;

namespace YiSA.WPF.Command
{
    public interface ICommand<T> : ICommand
    {
        public void Execute(T param);
        public bool CanExecute(T param);
    }
    
    public class DelegateCommand<T> : ICommand<T>
    {
        private readonly Action<T> _execute;
        protected Func<T,bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<T,bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? ((_)=>true);
        }

        public bool CanExecute(object parameter) => CanExecute((T) parameter);
        public bool CanExecute(T parameter) => _canExecute(parameter);
        public void Execute(object parameter) => Execute((T)parameter);
        public void Execute(T parameter) => _execute(parameter);


        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this,EventArgs.Empty);
        public event EventHandler? CanExecuteChanged;
    }
    
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
            : base(_ => execute(), 
                canExecute is null ? null : new Func<object, bool>(_ => canExecute()))
        {}
                   
        public void Execute() => Execute(null!);
    }
}