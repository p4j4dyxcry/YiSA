using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using YiSA.Markup.Animations;

namespace YiSA.Markup.Extension
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
    
    public class EasingExtension : MarkupExtension
    {
        private EaseType _easeType;
        public EasingExtension(EaseType easeType)
        {
            
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _easeType switch 
            {
                EaseType.Cubic => new CubicEasing(),
                EaseType.EaseIn => new EaseInEasing(),
                EaseType.EaseOut => new EaseOutEasing(),
                EaseType.EaseInBack => new EaseInBackEasing(),
                EaseType.EaseOutBack => new EaseOutBackEasing(),
                EaseType.EaseInOutBack => new EaseInOutBackEasing(),
                EaseType.EaseInElastic=> new EaseInElasticEasing(),
                EaseType.EaseOutElastic => new EaseOutElasticEasing(),
                EaseType.EaseInOutElastic => new EaseInOutElasticEasing(),
                _ => new CubicEasing()
            };
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