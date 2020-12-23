using System.Numerics;

namespace YiSA.Foundation.Common
{
    public enum YiStructureType : uint
    {
        Reference = 0,
        Struct    = 1,
    }
    
    public enum YiPropertyTypes
    {
        [BindType(typeof(int),YiStructureType.Struct)] 
        Int,
        [BindType(typeof(float),YiStructureType.Struct)] 
        Float,
        [BindType(typeof(bool),YiStructureType.Struct)] 
        Bool,
        [BindType(typeof(string),YiStructureType.Reference)] 
        String,
        [BindType(typeof(Vector2),YiStructureType.Struct)] 
        Vector2,
        [BindType(typeof(Vector3),YiStructureType.Struct)] 
        Vector3,
    }
}