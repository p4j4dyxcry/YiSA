using System;

namespace YiSA.Foundation.Common
{
    public class BindTypeAttribute : Attribute
    {
        public Type Type { get; }
        public YiStructureType YiStructureType { get; }
        public BindTypeAttribute(Type type, YiStructureType yiStructureType = default)
        {
            Type = type;
            YiStructureType = yiStructureType;
        }
    }
}