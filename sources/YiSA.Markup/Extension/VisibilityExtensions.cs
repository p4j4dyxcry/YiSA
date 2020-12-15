using System;
using System.Windows.Markup;
using YiSA.Markup.Common;
using YiSA.Markup.Converters;

namespace YiSA.Markup.Extension
{
    public class TrueToVisibleExtension: MarkupExtension
    {
        private static BoolToAnyConverter? _booleanToVisibilityConverter;
        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            return _booleanToVisibilityConverter ??= new BoolToAnyConverter()
            {
                True = Boxes.Visible,
                False = Boxes.Collapsed,
            };
        }
    }

    public class FalseToVisibleExtension: MarkupExtension
    {
        private static BoolToAnyConverter? _booleanToVisibilityConverter;
        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            return _booleanToVisibilityConverter ??= new BoolToAnyConverter()
            {
                True = Boxes.Collapsed,
                False = Boxes.Visible,
            };
        }
    }
}