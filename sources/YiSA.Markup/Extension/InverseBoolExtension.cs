using System;
using System.Windows.Markup;
using YiSA.Markup.Common;
using YiSA.Markup.Converters;

namespace YiSA.Markup.Extension
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