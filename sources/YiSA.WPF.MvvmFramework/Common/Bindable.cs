using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YiSA.WPF.Common
{
    public class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool TrySetProperty<T>(
            ref T source,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(source, value) is false)
            {
                return false;
            }

            source = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}