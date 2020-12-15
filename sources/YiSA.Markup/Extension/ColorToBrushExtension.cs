using System;
using System.Windows.Data;
using System.Windows.Markup;
using YiSA.Markup.Converters;

namespace YiSA.Markup.Extension
{
    public class ColorToBrushExtension : MarkupExtension
    {
        private static IValueConverter? _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new ColorToBrushConverter();
        }
    }
}