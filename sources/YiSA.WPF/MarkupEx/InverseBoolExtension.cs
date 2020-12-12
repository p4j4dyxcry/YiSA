using System;
using System.Windows.Markup;
using YiSA.WPF.Common;
using YiSA.WPF.Converters;

namespace YiSA.WPF.MarkupEx
{
    public class InverseBoolExtension: MarkupExtension
    {
        private static BoolToAnyConverter? _converter;
        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            return _converter ??= new BoolToAnyConverter()
            {
                True = Boxes.False,
                False = Boxes.True,
            };
        }
    }
}