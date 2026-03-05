using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class UIRelatedTextures : Resource
    {
        [MemoryArrayOffset(24, 24)] [XdbArray] public UITextureResource[] Items;

        public class UITextureResource : Resource
        {
            [MemoryOffset(12)] [XdbElement] public AsciiString Name;
            [MemoryOffset(4)] [XdbElement] public GenericField<Resource> TextureItem;
        }
    }
}
