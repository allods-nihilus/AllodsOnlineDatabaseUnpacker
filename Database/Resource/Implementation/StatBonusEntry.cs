using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    // Source: gameMechanics.constructor.schemes.item.ItemBonus$StatBonusEntry
    // Memory layout: 12 bytes per entry [4-byte padding, 4-byte value, 4-byte stat]
    public class StatBonusEntry : Resource
    {
        [MemoryOffset(8)] [XdbEnum(typeof(InnateStats))] public Int Stat;
        [MemoryOffset(4)] [XdbElement] public Int Value;
    }
}
