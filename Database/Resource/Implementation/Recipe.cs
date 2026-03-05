using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Recipe : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Int SkillScore;
        [MemoryOffset(40)] [XdbElement("Name")] public TextFileRef Name;
        [MemoryOffset(56)] [XdbElement("Description")] public TextFileRef Description;
        [MemoryOffset(72)] [XdbElement] public GenericField<Resource> Image;
    }
}
