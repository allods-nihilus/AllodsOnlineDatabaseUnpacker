using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class ItemMallBannerResource : Resource
    {
        [MemoryOffset(32)] [XdbElement("Description")] public TextFileRef Description;
        [MemoryOffset(48)] [XdbElement] public GenericField<Resource> Image;
    }
}
