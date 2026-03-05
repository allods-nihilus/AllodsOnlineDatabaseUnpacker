using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class PhysicMaterial : Resource
    {
        [MemoryOffset(24)] [XdbElement] public FileRef Parent;
        [MemoryOffset(32)] [XdbElement] public Float SfxParam;
        [MemoryOffset(36)] [XdbElement] public FileRef VisualSettings;
    }
}
