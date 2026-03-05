using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class BuffResource : Resource
    {
        // ImpactProducer inherited field
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> UiInfos;
        // Own fields - confirmed from hex dump + server XDB cross-reference
        [MemoryOffset(124)] [XdbElement] public AsciiString SysUIScriptName;
        [MemoryOffset(136)] [XdbElement] public Int StackLimit;
        [MemoryOffset(180)] [XdbElement] public FileRef Image;
        [MemoryOffset(204)] [XdbElement] public Int Duration;
        [MemoryOffset(208)] [XdbElement("Name")] public TextFileRef Name;
        [MemoryOffset(224)] [XdbElement("Description")] public TextFileRef Description;
    }
}
