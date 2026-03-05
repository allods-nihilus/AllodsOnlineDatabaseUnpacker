using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class SFXArmorSet : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Float WeaponSize;
        [MemoryOffset(32)] [XdbElement] public GenericField<Resource> WeaponItemClass;
        [MemoryOffset(36)] [XdbEnum(typeof(SFXMaterial))] public Int WeaponMaterial;
        [MemoryOffset(40)] [XdbEnum(typeof(SFXMaterial))] public Int ShieldMaterial;
        [MemoryOffset(44)] [XdbEnum(typeof(SFXMaterial))] public Int ArmorMaterial;
    }
}
