using System;
using System.Linq;
using System.Windows.Media.Animation;

namespace YiSA.WPF.Animations
{
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

    public class SnapEasing : IEasingFunction
    {
        private readonly int _step;
        private readonly double[] _table;
        private readonly IEasingFunction? _easingFunction;

        public SnapEasing(int step , IEasingFunction? easingFunction)
        {
            if(step <= 0)
                throw new ArgumentException(nameof(step));

            _step = step;
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