using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class MountResource : Resource
    {
        // Action inherited fields
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> UiInfos;
        [MemoryArrayOffset(28, 4)] [XdbArray] public Resource[] Groups;
        [MemoryOffset(44)] [XdbElement] public GenericField<Resource> ContextActionInfo;
        // Own fields - corrected via server XDB cross-reference
        [MemoryOffset(104)] [XdbElement] public GenericField<Resource> Image;
        [MemoryOffset(128)] [XdbElement] public Float SpeedDelta;
        [MemoryOffset(132)] [XdbElement] public Float Speed;
        [MemoryOffset(136)] [XdbElement] public Float Regen;
        [MemoryOffset(140)] [XdbElement] public Int PrepareDurationDelta;
        [MemoryOffset(144)] [XdbElement] public Int PrepareDuration;
        [MemoryOffset(148)] [XdbElement] public GenericField<Resource> VisualMount;
        [MemoryOffset(156)] [XdbElement] public Float HealthDelta;
        [MemoryOffset(160)] [XdbElement] public Float Health;
        [MemoryOffset(164)] [XdbElement] public Int Grade;
    }
}
