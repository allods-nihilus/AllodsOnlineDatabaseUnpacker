using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class AstralObject : Resource
    {
        [MemoryOffset(28)] [XdbElement] public FileRef FlareTexture;
        [MemoryOffset(36)] [XdbElement] public Float ForceFieldRadius;
        [MemoryOffset(40)] [XdbElement] public Float AstralRadius;
        [MemoryOffset(44)] [XdbElement] public Float MaxSize;
    }
}
