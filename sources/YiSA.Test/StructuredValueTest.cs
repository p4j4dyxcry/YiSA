using System.Numerics;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;
using YiSA.Foundation.Common;

namespace YiSA.Test
{
    public class StructuredValueTest : TestBase
    {
        public StructuredValueTest(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact(DisplayName = "byte サイズが意図した大きさになっていることを確認")]
        void ByteSizeCheck()
        {
            Assert.Equal(16,Unsafe.SizeOf<StructuredValue16>());
        }
        
        [Fact(DisplayName = "値の変更が連動していることを確認")]
        public void ValueChangedTest()
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

        [Fact(DisplayName = "浮動小数型の値が正しく設定できているかを確認")]
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

    }
}