using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Cue : Resource
    {
        [MemoryOffset(24)] [XdbElement] public TextFileRef Text;
        [MemoryOffset(40)] [XdbElement] public TextFileRef Name;
    }
}
