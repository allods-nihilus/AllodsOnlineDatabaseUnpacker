using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class UIRelatedTexts : Resource
    {
        [MemoryArrayOffset(24, 32)] [XdbArray] public UITextResource[] Items;

        public class UITextResource : Resource
        {
            [MemoryOffset(20)] [XdbElement] public AsciiString Name;
            [MemoryOffset(4)] [XdbElement] public AsciiString Resource;
        }
    }
}
