using System.Linq;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;
using YiSA.Foundation.Common;
using YiSA.Foundation.Logging;

namespace YiSA.Test
{
    public class SimpleWrapper
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
    
    
    public class StructuredValueTest : TestBase
    {
        public StructuredValueTest(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public void ByteWriteTest()
        {
            var value = new StructuredValue16
            {
                Bytes =
                {
                    _1 = 100,
                }
            };

            // 1byte目を書き換える

            // メンバーとして取得可能か検証
            Assert.Equal(100, value.AsInt);
            Assert.Equal(100, value.AsShort);
            Assert.Equal(100, value.AsLong);
            Assert.Equal(100, value.AsChar);

            // 浮動小数には暗黙的にキャストされない
            Assert.NotEqual(100, value.AsFloat);
            Assert.NotEqual(100, value.AsDouble);

            // ジェネリックで取得できるか検証
            Assert.Equal(100, value.Get<int>());
            Assert.Equal(100, value.Get<short>());
            Assert.Equal(100, value.Get<long>());
        }

        [Fact]
        public void FloatTest()
        {
            var value = new StructuredValue16();
            value.AsFloat = 0.5f;

            // float に設定した値が Vector2のXとして取得できるか
            Assert.Equal(0.5f, value.Get<Vector2>().X);

            // ジェネリックのsetの検証
            value.Set(0.75f);
            Assert.Equal(0.75f, value.Get<Vector2>().X);
            Assert.Equal(0.75f, value.Get<Vector3>().X);
            Assert.Equal(0.75f, value.AsFloat);

            // ジェネリックで構造体をセットする
            value.Set(new Vector3(0.25f, 0.5f, 0.75f));
            Assert.Equal(0.25f, value.Get<Vector2>().X);
            Assert.Equal(0.5f, value.Get<Vector2>().Y);

            Assert.Equal(0.25f, value.Get<Vector3>().X);
            Assert.Equal(0.5f, value.Get<Vector3>().Y);
            Assert.Equal(0.75f, value.Get<Vector3>().Z);

            Assert.Equal(0.25f, value.Get<Vector4>().X);
            Assert.Equal(0.5f, value.Get<Vector4>().Y);
            Assert.Equal(0.75f, value.Get<Vector4>().Z);

            Assert.Equal(0.25f, value.Get<float>());
        }

        private void a()
        {
            SimpleWrapper value = new SimpleWrapper();
            float sum = 0;
            for (int i = 0; i < 10000000; ++i)
            {
                value.Set<float>(i);
                sum += value.Get<float>() * 0.00001f;
            }

            Logger.WriteLine($"sum = {sum}", LogLevel.Default);
        }

        private void b()
        {
            var value = new StructuredValue16();

            float sum = 0;
            for (int i = 0; i < 10000000; ++i)
            {
                value.Set<float>(i);
                sum += value.Get<float>() * 0.00001f;
            }

            Logger.WriteLine($"sum = {sum}", LogLevel.Default);
        }

        private void c()
        {
            
            var value = new StructuredValue16();

            float sum = 0;
            for (int i = 0; i < 10000000; ++i)
            {
                value.AsFloat = i;
                sum += value.AsFloat * 0.00001f;
            }

            Logger.WriteLine($"sum = {sum}", LogLevel.Default);

        }
        
        [Fact]
        public void SpeedTest()
        {
            foreach (var _ in Enumerable.Range(0, 30))
            {
                using (ProfileTime("object"))
                    a();
                using (ProfileTime("generic"))
                    b();
                using (ProfileTime("direct"))
                    c();
            }
        }
    }
}