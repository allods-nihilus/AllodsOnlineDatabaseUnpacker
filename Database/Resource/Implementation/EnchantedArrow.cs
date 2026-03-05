using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class EnchantedArrow : Resource
    {
        [MemoryOffset(32)] [XdbElement("Name")] public TextFileRef Name;
        [MemoryOffset(48)] [XdbElement] public FileRef Image;
        [MemoryOffset(52)] [XdbElement] public FileRef VisualArrow;
        [MemoryOffset(56)] [XdbElement("Description")] public TextFileRef Description;
    }
}
