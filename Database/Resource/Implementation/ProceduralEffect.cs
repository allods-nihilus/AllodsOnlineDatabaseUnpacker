using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class ProceduralEffect : Resource
    {
        [MemoryOffset(36)] [XdbElement] public Int Color0;
        [MemoryOffset(32)] [XdbElement] public Int Color1;
        [MemoryOffset(28)] [XdbElement] public Int Color2;
        [MemoryOffset(24)] [XdbElement] public Int Color3;
    }
}
