using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Route : Resource
    {
        [MemoryOffset(45)] [XdbElement] public Bool DropAtEnd;
        [MemoryOffset(46)] [XdbElement] public Bool ShowStartFog;
        [MemoryOffset(47)] [XdbElement] public Bool ShowEndFog;
    }
}
