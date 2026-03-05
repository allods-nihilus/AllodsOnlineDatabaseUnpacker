using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class AlternativeCurrency : Resource
    {
        [MemoryOffset(24)] [XdbElement] public AsciiString SysName;
        [MemoryOffset(36)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(52)] [XdbElement] public GenericField<Resource> Image;
        [MemoryOffset(60)] [XdbElement] public TextFileRef Description;
    }
}
