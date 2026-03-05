using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    // Source: gameMechanics.constructor.schemes.item.ItemResource
    // Base: gameMechanics.constructor.basicInterfaces.ImpactProducer (ImpactProducer)
    public class ItemResource : Resource
    {
        // Confirmed field offsets from memory probing
        [MemoryOffset(84)] [XdbEnum(typeof(HonorRank))] public Int RequiredHonor;
        [MemoryOffset(88)] [XdbEnum(typeof(ReputationLevel))] public Int RequiredReputation;
        [MemoryOffset(92)] [XdbElement] public FileRef VisualElement;
        [MemoryOffset(108)] [XdbElement] public AsciiString SysName;
        [MemoryOffset(120)] [XdbElement] public Int StackLimit;
        [MemoryOffset(124)] [XdbElement] public FileRef Spell;
        [MemoryOffset(132)] [XdbEnum(typeof(DressSlot))] public Int Slot;
        [MemoryOffset(136)] [XdbElement] public Int SellPrice;
        [MemoryOffset(144)] [XdbElement] public Int RequiredLevel;
        [MemoryOffset(152)] [XdbElement] public FileRef Quality;
        [MemoryOffset(164)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(184)] [XdbElement] public Int Level;
        [MemoryOffset(208)] [XdbElement] public FileRef ItemClass;
        [MemoryOffset(224)] [XdbElement] public FileRef Image;
        [MemoryOffset(232)] [XdbElement] public GenericField<Resource> FunctionalPart;
        [MemoryOffset(236)] [XdbElement] public FileRef Upgrade;
        [MemoryOffset(252)] [XdbElement] public TextFileRef Description;
        [MemoryOffset(272)] [XdbElement] public Int OwnershipLimit;
        [MemoryOffset(284)] [XdbElement] public Bool NameChecked;
        [MemoryOffset(285)] [XdbElement] public Bool IncludeInBoxes;
        [MemoryOffset(286)] [XdbElement] public Bool CustomSellPrice;
        [MemoryOffset(287)] [XdbElement] public Bool CustomBuyPrice;
        [MemoryOffset(288)] [XdbElement] public Int BuyPrice;
        [MemoryOffset(292)] [XdbEnum(typeof(Binding))] public Int Binding;
    }
}
