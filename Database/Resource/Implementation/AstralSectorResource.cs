using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class AstralSectorResource : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Int TechLevel;
        [MemoryOffset(28)] [XdbElement("Name")] public TextFileRef Name;
    }
}
