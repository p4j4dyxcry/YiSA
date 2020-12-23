using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using YiSA.Foundation.Common;
using YiSA.Foundation.Logging;

namespace YiSA.Test
{
    public class StructuredValuePerformance : TestBase
    {
        // ctor
        public StructuredValuePerformance(ITestOutputHelper helper) : base(helper)
        { }

        private void FlotGetAndSetPattern(Func<float> getter ,Action<float> setter , int loop)
        {
            float value = 0;
            for (int i = 0; i < loop; ++i)
            {
                setter(0.5f);
                value = getter();
            }

            Assert.Equal(0.5f,value);
        }

        private void Dummy()
        {
            // キャッシュとかが計測に入らないように1度実行しておく
            foreach (var _ in Enumerable.Range(0, 3))
            {
                using (ProfileTime("dummy"))
                {
                    var x = new BoxingWrapper();
                    FlotGetAndSetPattern(() => x.Get<float>() , v => x.Set(v) ,10000);
                    
                    var y = new StructuredValue16();
                    FlotGetAndSetPattern( () => y.Get<float>() , v => y.Set(v) ,10000);

                    var z = new StructuredValue16();
                    FlotGetAndSetPattern( () => z.AsFloat , v => z.AsFloat = v ,10000);                   
                }
            }
        }
        
        [Fact(DisplayName = "StructuredValue16を利用したフィールドアクセスがboxingより優れているかを検証")]
        public void PerformanceCheck()
        {
            int[] loops = 
            {
                5000,
                200000,
                400000000,
            };

            Dummy();
            
            var boxingWrapper = new BoxingWrapper();
            var structuredValue16 = new StructuredValue16();

            Action<float> boxingSet = x  => boxingWrapper.Set<float>(x);
            Func  <float> boxingGet = () => boxingWrapper.Get<float>();
            
            Action<float> genricSet = x  => structuredValue16.Set<float>(x);
            Func  <float> genericGet = () => structuredValue16.Get<float>();
            
            Action<float> dynamicGet = x  => structuredValue16.AsFloat = x;
            Func  <float> dynamicSet = () => structuredValue16.AsFloat;
            
            foreach (var loop in loops)
            {
                using (ProfileTime($"object {loop}"))
                {
                    FlotGetAndSetPattern(boxingGet , boxingSet ,loop);
                }

                using (ProfileTime($"generic {loop}"))
                {
                    
                    FlotGetAndSetPattern( genericGet , genricSet ,loop);
                }

                using (ProfileTime($"direct {loop}"))
                {
                    FlotGetAndSetPattern( dynamicSet , dynamicGet ,loop);                   
                }
            }
        }
        
        public class BoxingWrapper
        {
            private object obj = null;
            public T Get<T>()
            {
                return (T) obj;
            }
            public void Set<T>(T value)
            {
                obj = value;
            }
        }
    }
}