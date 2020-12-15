using System.Windows;

namespace YiSA.Markup.MethodExtensions
{
    public static class FreezableExtension
    {
        public static T DoFreeze<T>(this T @this) where T : Freezable
        {
            if (@this.CanFreeze & @this.IsFrozen is false)
                @this.Freeze();
            return @this;
        }
    }
}