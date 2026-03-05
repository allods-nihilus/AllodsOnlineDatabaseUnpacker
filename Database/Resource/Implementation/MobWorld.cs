using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class MobWorld : Resource
    {
        // Inherited from RespawnableResource
        [MemoryOffset(60)] [XdbElement] public Float Corpulence;
        // Own fields
        [MemoryOffset(76)] [XdbElement] public Bool UsesWeapon;
        [MemoryOffset(108)] [XdbElement] public GenericField<Resource> Quality;
        [MemoryOffset(132)] [XdbElement] public Int LevelMin;
        [MemoryOffset(144)] [XdbElement] public GenericField<Resource> Title;
    }
}
