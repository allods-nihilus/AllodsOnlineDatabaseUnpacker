using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Proficiency : Resource
    {
        [MemoryOffset(24)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(40)] [XdbElement] public GenericField<Resource> Image;
        [MemoryOffset(48)] [XdbElement] public TextFileRef Description;
    }
}
