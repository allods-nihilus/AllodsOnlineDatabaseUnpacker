using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    // Source: gameMechanics.constructor.schemes.item.ItemBonus
    // Base: gameMechanics.constructor.schemes.item.ItemFunctionalPart
    public class ItemBonus : Resource
    {
        [MemoryOffset(76)] [XdbElement] public Int Armor;
        [MemoryArrayOffset(28, 12)] [XdbArray] public StatBonusEntry[] StatBonuses;
        [MemoryOffset(60)] [XdbElement] public Int MinDamage;
        [MemoryOffset(64)] [XdbElement] public Int MaxDamage;
        [MemoryOffset(24)] [XdbElement] public Float WeaponSpeed;
        [MemoryOffset(44)] [XdbElement] public Int SpellPower;
        [MemoryOffset(40)] [XdbEnum(typeof(SubElement))] public Int DamageElement;
        [MemoryOffset(48)] [XdbElement] public Int ResistDivine;
        [MemoryOffset(52)] [XdbElement] public Int ResistElemental;
        [MemoryOffset(56)] [XdbElement] public Int ResistNature;
    }
}
