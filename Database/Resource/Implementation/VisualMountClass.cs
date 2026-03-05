using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class VisualMountClass : Resource
    {
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> AnimationSettings;
        [MemoryOffset(28)] [XdbElement] public Int FxSettings;
        [MemoryOffset(32)] [XdbElement] public GenericField<Resource> ControlParameters;
        [MemoryOffset(36)] [XdbElement] public GenericField<Resource> PitchParameters;
        [MemoryOffset(40)] [XdbElement] public GenericField<Resource> JumpSettings;
        [MemoryOffset(44)] [XdbElement] public Int ReinsSettings;
        [MemoryOffset(48)] [XdbElement] public AsciiString LevelChangedScript;
        [MemoryOffset(60)] [XdbElement] public GenericField<Resource> SummonVisualScript;
        [MemoryOffset(64)] [XdbElement] public GenericField<Resource> SitAnimation;
        [MemoryOffset(68)] [XdbElement] public GenericField<Resource> DismountAnimation;
    }
}
