using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YiSA.Foundation.Common
{
    /// <summary>
    /// 16byteの汎用構造体です。
    /// example
    ///  var value = new StructuredValue16();
    ///  var intVal = value.Get<int>();
    ///  value.Set<float>(30f);
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct StructuredValue16
    {
        // 本体
        [FieldOffset(0)] public Byte16  Bytes;
        
        // Byte列をそれぞれの型として値を取り出します。
        // 注意:この取り出しでは int -> float等は暗黙的にキャストされません。
        [FieldOffset(0)] public int     AsInt;    // 最初の4byteをintとして取り出します。
        [FieldOffset(0)] public bool    AsBool;   // 最初の1byteをboolとして取り出します。
        [FieldOffset(0)] public float   AsFloat;  // 最初の4byteをfloatとして取り出します。
        [FieldOffset(0)] public double  AsDouble; // 最初の8byteをdoubleとして取り出します。
        [FieldOffset(0)] public short   AsShort;  // 最初の2byteをshortとして取り出します。
        [FieldOffset(0)] public long    AsLong;   // 最初の8byteをlongとして取り出します。
        [FieldOffset(0)] public uint    AsIntU;   // 最初の4byteをunsigned intとして取り出します。
        [FieldOffset(0)] public ulong   AsLongU;  // 最初の8byteをunsigned longとして取り出します。
        [FieldOffset(0)] public ushort  AsShortU; // 最初の2byteをunsigned shortとして取り出します。
        [FieldOffset(0)] public byte    AsByte;   // 最初の1byteをbyteして取り出します。
        [FieldOffset(0)] public sbyte   AsByteS;  // 最初の1byteをsinged byteして取り出します。
        [FieldOffset(0)] public char    AsChar;   // 最初の1byteをcharして取り出します。
        [FieldOffset(0)] public decimal AsDecimal;// 16byteをdecimalして取り出します。
        [FieldOffset(0)] public Vector2 AsVector2;// 最初の8byteをvector2して取り出します。
        [FieldOffset(0)] public Vector3 AsVector3;// 最初の12byteをvector3して取り出します。
        [FieldOffset(0)] public Vector4 AsVector4;// 16byteをvector4して取り出します。

        public float this[int i]
        {
            get
            {
                return i switch
                {
                    1 => AsVector4.X,
                    2 => AsVector4.Y,
                    3 => AsVector4.Z,
                    4 => AsVector4.W,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
            set
            {
                switch (i)
                {
                    case 1: AsVector4.X = value;break;
                    case 2: AsVector4.Y = value;break;
                    case 3: AsVector4.Z = value;break;
                    case 4: AsVector4.W = value;break;
                    default  : throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// genericで値を取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : struct
        {
            return StructuredValue16GenericCache<T>.Get(ref this);
        }
        public void Set<T>(T value) where T : struct
        {
            StructuredValue16GenericCache<T>.Set(ref this,ref value);
        }
    }

    /// <summary>
    /// 特定の値のアクセサを取得するためのキャッシュです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StructuredValue16GenericCache<T>
    {
        private static readonly int Size;

        /// <summary>
        /// 初期化時にTのサイズを計算します。
        /// </summary>
        static StructuredValue16GenericCache()
        {
            Size = Unsafe.SizeOf<T>();
        }
        
        /// <summary>
        /// Tのサイズに合わせた最適な取得関数をつかって値を取得します。
        /// 対応していない場合はNotSupportedExceptionがthrowされます。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Get(ref StructuredValue16 value)
        {
            switch (Size)
            {
                case 1  : return Unsafe.As<byte   ,T>(ref value.AsByte);
                case 2  : return Unsafe.As<short  ,T>(ref value.AsShort);                
                case 4  : return Unsafe.As<int    ,T>(ref value.AsInt);
                case 8  : return Unsafe.As<long   ,T>(ref value.AsLong);
                case 12 : return Unsafe.As<Vector3,T>(ref value.AsVector3);
                case 16 : return Unsafe.As<Byte16 ,T>(ref value.Bytes);
                default : throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Tのサイズに合わせた最小限のデータのセットを行います。
        /// 対応していない場合はNotSupportedExceptionがthrowされます。
        /// </summary>
        /// <param name="result"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void Set(ref StructuredValue16 result , ref T value)
        {
            switch (Size)
            {
                case   1: result.AsByte     = Unsafe.As<T, byte>   (ref value); break;
                case   2: result.AsShort    = Unsafe.As<T, short>  (ref value); break;                
                case   4: result.AsInt      = Unsafe.As<T, int>    (ref value); break;
                case   8: result.AsLong     = Unsafe.As<T, long>   (ref value); break;
                case  12: result.AsVector3  = Unsafe.As<T, Vector3>(ref value); break;
                case  16: result.Bytes      = Unsafe.As<T, Byte16> (ref value); break;
                default : throw new NotSupportedException();
            }
        }
    }

    // Byteの固定配列型です。
    [StructLayout(16)]
    public struct Byte16
    {
        public byte _1;
        public byte _2;
        public byte _3;
        public byte _4;
        public byte _5;
        public byte _6;
        public byte _7;
        public byte _8;
        public byte _9;
        public byte _10;
        public byte _11;
        public byte _12;
        public byte _13;
        public byte _14;
        public byte _15;
        public byte _16;

        public byte[] ToArray()
        {
            return new[]
            {
                _1 , _2 ,_3 ,_4 ,
                _5 , _6 ,_7 ,_8 ,
                _9 , _10,_11,_12,
                _13, _14,_15,_16,
            };
        }
    }
}