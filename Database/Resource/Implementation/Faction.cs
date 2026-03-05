using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Faction : Resource
    {
        [MemoryOffset(36)] [XdbElement] public AsciiString SysTutorialName;
        [MemoryOffset(48)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(64)] [XdbElement] public Bool LittleOldMan;
        [MemoryOffset(68)] [XdbElement] public FileRef ThoughtsPool;
        [MemoryOffset(84)] [XdbElement] public Int DefaultReputation;
    }
}
