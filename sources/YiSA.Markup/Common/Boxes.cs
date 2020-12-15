using System.Windows;

namespace YiSA.Markup.Common
{
    public static class Boxes
    {
        public static object True       { get; } = true;
        public static object False      { get; } = false;
        
        public static object ZeroD { get; } = 0.0d;
        public static object ZeroF { get; } = 0.0f;
        public static object ZeroL { get; } = 0L;
        public static object Zero  { get; } = 0;
        public static object ZeroU { get; } = 0U;

        public static object Visible   { get; } = Visibility.Visible;
        public static object Collapsed { get; } = Visibility.Collapsed;
        public static object Hidden    { get; } = Visibility.Hidden;
    }
}