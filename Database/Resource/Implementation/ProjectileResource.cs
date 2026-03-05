using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class ProjectileResource : Resource
    {
        // RespawnableResource > BunchResource inherited fields
        [MemoryArrayOffset(24, 4)] [XdbArray] public Resource[] PersistentParts;
        [MemoryArrayOffset(36, 4)] [XdbArray] public Resource[] Parts;
        // Own fields
        [MemoryOffset(76)] [XdbElement] public Float Corpulence;
    }
}
