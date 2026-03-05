using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class ZoneLights : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Float WaterReflectionCoefficient;
        [MemoryOffset(28)] [XdbElement] public FileRef WaterLight;
    }
}
