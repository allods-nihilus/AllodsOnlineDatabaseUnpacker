using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class CameraShakeParameters : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Float AmplitudeScale;
        [MemoryOffset(28)] [XdbElement] public Float MinRadius;
        [MemoryOffset(32)] [XdbElement] public Float MaxRadius;
        [MemoryOffset(36)] [XdbElement] public Bool Looped;
        [MemoryOffset(40)] [XdbElement] public FileRef Animation;
        [MemoryOffset(48)] [XdbElement] public Float TimeScale;
    }
}
