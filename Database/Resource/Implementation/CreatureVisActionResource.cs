using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class CreatureVisActionResource : Resource
    {
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> Action;
        [MemoryOffset(28)] [XdbElement] public TextFileRef Description;
    }
}
