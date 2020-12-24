using Reactive.Bindings;
using YiSA.WPF.Common;

namespace YiSA.WPF.ViewModels
{
    public class IntPrimitiveViewModel : DisposableBindable , IPrimitiveViewModel<int>
    {
        public IReactiveProperty<int> ReactiveProperty { get; }
    }
}