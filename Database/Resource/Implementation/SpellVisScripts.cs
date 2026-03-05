using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class SpellVisScripts : Resource
    {
        // Inherited from ActionVisScripts (same layout as ExploitVisScripts)
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> ActionBarStart;
        [MemoryOffset(28)] [XdbElement] public GenericField<Resource> WholeCasting;
        [MemoryOffset(32)] [XdbElement] public GenericField<Resource> ActionBarComplite;
        [MemoryOffset(36)] [XdbElement] public GenericField<Resource> ActionBarCancel;
        [MemoryOffset(40)] [XdbElement] public GenericField<Resource> PrecastType;
        [MemoryOffset(44)] [XdbElement] public TextFileRef Description;
        // Own fields
        [MemoryOffset(72)] [XdbElement] public GenericField<Resource> Launch;
        [MemoryOffset(92)] [XdbElement] public GenericField<Resource> Charged;
        [MemoryOffset(96)] [XdbElement] public GenericField<Resource> PlayOnTargets;
    }
}
