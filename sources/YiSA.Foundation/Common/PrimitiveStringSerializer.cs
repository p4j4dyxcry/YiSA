using System.Linq;
using System.Numerics;
using System.Text;
using YiSA.Foundation.Logging;

namespace YiSA.Foundation.Common
{
    interface IPrimitiveStringSerializer<T>
    {
        public string Serialize(ref T value);
        public T Deserialize(string value , T fallback);
    }

    public class SerializerBase<T> : IPrimitiveStringSerializer<T>
    {
        private readonly ILogger? _logger;

        public SerializerBase(ILogger? logger)
        {
            _logger = logger;
        }

        public string Serialize(ref T value)
        {
            return SerializeImpl(ref value);
        }

        public T Deserialize(string value , T fallback)
        {
            if (DeserializeImpl(value, out var result))
                return result;

            _logger?.WriteLine($"parse failed. $\"{value}\" to {typeof(T).Name}",LogLevel.Warning);
            return fallback;
        }

        protected virtual string SerializeImpl(ref T value)
        {
            return value?.ToString() ?? string.Empty;
        }
        
        protected virtual bool DeserializeImpl(string value , out T result)
        {
            result = default!;
            return false;
        }
    }

    /// <summary>
    /// Bool ＜ー＞ String
    /// </summary>
    public class BoolStringSerializer : SerializerBase<bool>
    {
        public BoolStringSerializer(ILogger? logger) : base(logger){}

        protected override bool DeserializeImpl(string value, out bool result)
            => bool.TryParse(value, out result);
    }
    
    /// <summary>
    /// String ＜ー＞ String
    /// </summary>
    public class StringStringSerializer : SerializerBase<string>
    {
        public StringStringSerializer(ILogger? logger) : base(logger){}

        protected override bool DeserializeImpl(string value, out string result)
        {
            result = value;
            return true;
        }
    }
    
    /// float ＜ー＞ string
    public class FloatStringSerializer : SerializerBase<float>
    {
        public FloatStringSerializer(ILogger? logger) : base(logger){}

        protected override bool DeserializeImpl(string value, out float result)
            => float.TryParse(value, out result);

        
        /// <summary>
        /// 配列用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal string SerializeArray(float value , params float[] values)
        {
            var builder = new StringBuilder();

            builder.Append(value);
            for (int i = 0; i < values.Length; ++i)
            {
                builder.Append(',');
                builder.Append(values[i]);                
            }

            return builder.ToString();
        }

        /// <summary>
        /// 配列用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool DeserializeArray(string value, out StructuredValue16 result)
        {
            result = new StructuredValue16();

            var values = value.Split(',').Select(x => x.Trim()).ToArray();

            if (value.Any() is false)
                return false;
            
            int i = 0;
            foreach (var f in values)
            {
                if (float.TryParse(f, out var c))
                    result[i++] = c;
                else
                    return false;
            }

            return true;
        }
    }
    
    /// int ＜ー＞ string
    public class IntStringSerializer : SerializerBase<int>
    {
        public IntStringSerializer(ILogger? logger) : base(logger){}

        protected override bool DeserializeImpl(string value, out int result)
            => int.TryParse(value, out result);
    }

    /// Vector2 ＜ー＞ string
    public class Vector2StringSerializer : SerializerBase<Vector2>
    {
        private readonly FloatStringSerializer _floatStringSerializer;

        public Vector2StringSerializer(ILogger? logger) : base(logger)
        {
            _floatStringSerializer = new FloatStringSerializer(logger);
        }

        protected override string SerializeImpl(ref Vector2 value)
        {
            return _floatStringSerializer.SerializeArray(value.X, value.Y);
        }

        protected override bool DeserializeImpl(string value, out Vector2 result)
        {
            var success = _floatStringSerializer.DeserializeArray(value, out var c);
            result = c.AsVector2;
            return success;
        }
    }
    
    /// Vector3 ＜ー＞ string
    public class Vector3StringSerializer : SerializerBase<Vector3>
    {
        private readonly FloatStringSerializer _floatStringSerializer;

        public Vector3StringSerializer(ILogger? logger) : base(logger)
        {
            _floatStringSerializer = new FloatStringSerializer(logger);
        }

        protected override string SerializeImpl(ref Vector3 value)
        {
            return _floatStringSerializer.SerializeArray(value.X, value.Y,value.Z);
        }

        protected override bool DeserializeImpl(string value, out Vector3 result)
        {
            var success = _floatStringSerializer.DeserializeArray(value, out var c);
            result = c.AsVector3;
            return success;
        }
    }
    
    /// <summary>
    /// Vector4 ＜ー＞ string
    /// </summary>
    public class Vector4StringSerializer : SerializerBase<Vector4>
    {
        private readonly FloatStringSerializer _floatStringSerializer;

        public Vector4StringSerializer(ILogger? logger) : base(logger)
        {
            _floatStringSerializer = new FloatStringSerializer(logger);
        }

        protected override string SerializeImpl(ref Vector4 value)
        {
            return _floatStringSerializer.SerializeArray(value.X, value.Y,value.Z,value.W);
        }

        protected override bool DeserializeImpl(string value, out Vector4 result)
        {
            var success = _floatStringSerializer.DeserializeArray(value, out var c);
            result = c.AsVector4 ;
            return success;
        }
    }
}