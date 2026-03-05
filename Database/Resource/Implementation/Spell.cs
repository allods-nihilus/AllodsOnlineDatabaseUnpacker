using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class Spell : Resource
    {
        // Action inherited fields (uiInfos, groups, contextActionInfo)
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> UiInfos;
        [MemoryArrayOffset(28, 4)] [XdbArray] public Resource[] Groups;
        [MemoryOffset(36)] [XdbElement] public GenericField<Resource> ContextActionInfo;
        // Spell "both" visibility fields
        [MemoryOffset(156)] [XdbElement] public Float Range;
        [MemoryOffset(160)] [XdbElement] public Int PrepareDuration;
        [MemoryOffset(180)] [XdbElement] public GenericField<Resource> Mechanics;
        // Spell "client" visibility fields
        [MemoryOffset(232)] [XdbElement] public TextFileRef Name;
        [MemoryOffset(248)] [XdbElement] public TextFileRef Description;
        [MemoryOffset(264)] [XdbElement] public Int DefaultAction;
    }
}
