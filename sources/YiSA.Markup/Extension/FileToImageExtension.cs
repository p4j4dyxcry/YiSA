using System;
using System.Windows.Markup;
using YiSA.Markup.Converters;

namespace YiSA.Markup.Extension
{
    public class FileToImageExtension: MarkupExtension
    {
        private static FilePathToImageConverter? _converter;
        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            return _converter ??= new FilePathToImageConverter();
        }
    }
}