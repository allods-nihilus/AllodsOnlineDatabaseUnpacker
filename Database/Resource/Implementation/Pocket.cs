using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    // Source: gameMechanics.constructor.schemes.item.Pocket
    // Native size: 52 bytes per entry
    // TextFileRef occupies 16 bytes in native memory (begin+end+capacity+resourceID)
    public class Pocket : Resource
    {
        [MemoryOffset(0)] [XdbElement] public GenericField<Resource> Condition;
        [MemoryOffset(4)] [XdbElement] public Int Size;
        [MemoryOffset(8)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(24)] [XdbElement] public FileRef Image;
        [MemoryOffset(32)] [XdbElement] public TextFileRef Description;
    }
}
