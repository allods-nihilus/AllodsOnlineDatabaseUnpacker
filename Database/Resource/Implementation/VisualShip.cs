using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class VisualShip : Resource
    {
        // BunchResource inherited fields
        [MemoryArrayOffset(24, 4)] [XdbArray] public Resource[] PersistentParts;
        [MemoryArrayOffset(36, 4)] [XdbArray] public Resource[] Parts;
        // Own fields
        [MemoryOffset(60)] [XdbElement] public Int SourceFileCRC;
        [MemoryOffset(80)] [XdbElement] public Float Radius;
    }
}
