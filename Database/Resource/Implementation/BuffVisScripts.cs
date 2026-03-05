using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class BuffVisScripts : Resource
    {
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> PostAction;
        [MemoryOffset(28)] [XdbElement] public GenericField<Resource> Action;
        [MemoryOffset(32)] [XdbElement] public TextFileRef Description;
    }
}
