using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Character : Resource
    {
        [MemoryOffset(24)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(40)] [XdbElement] public FileRef Faction;
        [MemoryArrayOffset(48, 16)] [XdbArray] public DressItemEntry[] DressItems;
        [MemoryOffset(96)] [XdbElement] public AsciiString ChargenAnimationStart;
        [MemoryOffset(108)] [XdbElement] public AsciiString ChargenAnimationLoop;
        [MemoryOffset(120)] [XdbElement] public FileRef CharacterSex;
        [MemoryOffset(128)] [XdbElement] public FileRef CharacterRaceClass;

        public class DressItemEntry : Resource
        {
            [MemoryOffset(4)] [XdbEnum(typeof(DressSlot))] public Int Slot;
            [MemoryOffset(8)] [XdbElement("item")] public FileRef Item;
        }
    }
}
