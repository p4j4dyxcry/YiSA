using System;
using System.Windows.Data;
using System.Windows.Markup;
using YiSA.WPF.Converters;

namespace YiSA.WPF.MarkupEx
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