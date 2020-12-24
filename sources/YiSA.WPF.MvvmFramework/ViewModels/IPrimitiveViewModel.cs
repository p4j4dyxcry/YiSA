using System.ComponentModel;
using Reactive.Bindings;

namespace YiSA.WPF.ViewModels
{
    public interface IPrimitiveViewModel<T> : INotifyPropertyChanged
    {
        public IReactiveProperty<T> ReactiveProperty { get; }
        public T Value => ReactiveProperty.Value;
    }
}