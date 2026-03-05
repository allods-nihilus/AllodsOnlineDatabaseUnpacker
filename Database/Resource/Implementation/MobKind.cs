using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class MobKind : Resource
    {
        [MemoryOffset(32)] [XdbEnum(typeof(Race))] public Int Race;
        [MemoryOffset(36)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(52)] [XdbEnum(typeof(ManaType))] public Int ManaType;
        [MemoryOffset(56)] [XdbElement] public AsciiString ClassName;
        [MemoryOffset(68)] [XdbElement] public GenericField<Resource> AttackRangeStats;
    }
}
