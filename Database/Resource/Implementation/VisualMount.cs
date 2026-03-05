using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class VisualMount : Resource
    {
        [MemoryOffset(24)] [XdbElement] public FileRef VisualMountClass;
        [MemoryOffset(32)] [XdbElement] public FileRef Mount;
        [MemoryOffset(48)] [XdbElement] public Float MountSize;
    }
}
