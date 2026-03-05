using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class ShipResource : Resource
    {
        // BunchResource inherited fields
        [MemoryArrayOffset(24, 4)] [XdbArray] public Resource[] PersistentParts;
        [MemoryArrayOffset(36, 4)] [XdbArray] public Resource[] Parts;
        // Own fields
        [MemoryOffset(60)] [XdbElement] public Float Mass;
        [MemoryOffset(64)] [XdbElement] public GenericField<Resource> ShipClass;
        [MemoryOffset(72)] [XdbElement] public Int TechLevel;
    }
}
