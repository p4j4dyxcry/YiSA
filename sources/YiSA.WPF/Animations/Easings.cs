using System;
using System.Linq;
using System.Windows.Media.Animation;

namespace YiSA.WPF.Animations
{
    public enum EaseType
    {
        Cubic,
        EaseIn,
        EaseOut,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
    }
    
    public class CubicEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            return t * t * (3 - 2 * t);
        }
    }
    
    public class EaseInEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            return t * t;
        }
    }
    
    public class EaseOutEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            return t * ( 2 - t );
        }
    }
    public class EaseInElasticEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            const double c4 = (2 * Math.PI) / 3;

            if (t is 0)
                return 0;

            if (Math.Abs(t - 1) < 0.0001d)
                return 1;
            
            return -Math.Pow(2, 10 * t - 10) * Math.Sin((t * 10 - 10.75) * c4);
        }
    }

    public class EaseOutElasticEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            const double c4 = (2 * Math.PI) / 3;

            if (t is 0)
                return 0;

            if (Math.Abs(t - 1) < 0.0001d)
                return 1;
            
            return Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1;
        }
    }

    public class EaseInOutElasticEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            const double c5 = (2 * Math.PI) / 4.5;

            if (t is 0)
                return 0;

            if (Math.Abs(t - 1) < 0.0001d)
                return 1;

            if (t < 0.5)
            {
                return -(Math.Pow(2, 20 * t - 10) * Math.Sin((20 * t - 11.125) * c5)) / 2;
            }
            
            return (Math.Pow(2, -20 * t + 10) * Math.Sin((20 * t - 11.125) * c5)) / 2 + 1;
        }
    }

    public class EaseInBackEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return c3 * t * t * t - c1 * t * t;
        }
    }

    public class EaseOutBackEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return 1 + c3 * Math.Pow(t - 1, 3) + c1 * Math.Pow(t - 1, 2);
        }
    }

    public class EaseInOutBackEasing : IEasingFunction
    {
        public double Ease(double t)
        {
            const double c1 = 1.70158;
            const double c2 = c1 * 1.525;

            if (t < 0.5)
            {
                return (Math.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2;
            }
            
            return (Math.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }
    }

    public class SnapEasing : IEasingFunction
    {
        private readonly double[] _table;
        private readonly IEasingFunction? _easingFunction;

        public SnapEasing(int step , IEasingFunction? easingFunction)
        {
            if(step <= 0)
                throw new ArgumentException(nameof(step));

            _easingFunction = easingFunction;
            _table = new double[step + 1];
            _table[0] = 0d;
            _table[step] = 1d;

            var unit = 1.0d / step;
            for(var i = 1; i < step; ++i)
                _table[i] = i * unit;
        }
        
        public double Ease(double t)
        {
            if(_easingFunction != null)
                t = _easingFunction.Ease(t);
            return _table.OrderBy(x => Math.Abs(t - x)).First();
        }
    }
}