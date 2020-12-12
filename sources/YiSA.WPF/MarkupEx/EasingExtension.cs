using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using YiSA.WPF.Animations;

namespace YiSA.WPF.MarkupEx
{
    public class SnapEasingExtension : MarkupExtension
    {
        private readonly int _step;
        public SnapEasingExtension(int step)
        {
            _step = step;
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new SnapEasing(_step,new CubicEasing());
        }
    }
    
    public class CubicEasingExtension : MarkupExtension
    {
        private readonly IEasingFunction _easing = new CubicEasing();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _easing;
        }
    }
    
    public class EaseInEasingExtension : MarkupExtension
    {
        private readonly IEasingFunction _easing = new EaseInEasing();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _easing;
        }
    }
    
    public class EaseOutEasingExtension : MarkupExtension
    {
        private readonly IEasingFunction _easing = new EaseOutEasing();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _easing;
        }
    }
}