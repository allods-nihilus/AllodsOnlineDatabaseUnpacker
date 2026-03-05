using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class CharacterClass : Resource
    {
        [MemoryOffset(24)] [XdbElement("Name")] public TextFileRef Name;
        [MemoryOffset(52)] [XdbElement] public Int ManaDice;
        [MemoryOffset(88)] [XdbElement] public Float HitDice;
        [MemoryOffset(92)] [XdbElement] public AsciiString ClassName;
        [MemoryOffset(104)] [XdbElement] public TextFileRef UiName;
    }
}
