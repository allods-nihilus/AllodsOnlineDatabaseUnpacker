using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class CreatureVisState : Resource
    {
        [MemoryOffset(40)] [XdbElement] public Int StateID;
    }
}
