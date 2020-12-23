using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using YiSA.Markup.Animations;

namespace YiSA.Markup.Extension
{
    public class SnapEasingExtension : MarkupExtension
    {
        private readonly int _step;
        private readonly EaseType _easeType;
        private readonly EasingExtension _easingExtension;

        public SnapEasingExtension(int step , EaseType easeType)
        {
            _step = step;
            _easeType = easeType;
            _easingExtension = new EasingExtension(easeType);
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var easing = _easingExtension.ProvideValue(serviceProvider) as IEasingFunction;
            return new SnapEasing(_step,easing);
        }
    }
    
    public class EasingExtension : MarkupExtension
    {
        private EaseType _easeType;
        public EasingExtension(EaseType easeType)
        {
            _easeType = easeType;
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _easeType switch 
            {
                EaseType.None => NoneEasing.Default,
                EaseType.Cubic => new CubicEasing(),
                EaseType.EaseIn => new EaseInEasing(),
                EaseType.EaseOut => new EaseOutEasing(),
                EaseType.EaseInBack => new EaseInBackEasing(),
                EaseType.EaseOutBack => new EaseOutBackEasing(),
                EaseType.EaseInOutBack => new EaseInOutBackEasing(),
                EaseType.EaseInElastic => new EaseInElasticEasing(),
                EaseType.EaseOutElastic => new EaseOutElasticEasing(),
                EaseType.EaseInOutElastic => new EaseInOutElasticEasing(),
                _ => NoneEasing.Default,
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