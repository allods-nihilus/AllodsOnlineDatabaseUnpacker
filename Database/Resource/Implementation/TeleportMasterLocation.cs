using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class TeleportMasterLocation : Resource
    {
        [MemoryOffset(24)] [XdbElement] public TextFileRef Name;
    }
}
