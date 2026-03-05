using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class UserPostEffect : Resource
    {
        [MemoryOffset(24)] [XdbElement] public FileRef TextureAdditive;
        [MemoryOffset(32)] [XdbElement] public FileRef TextureMultiply;
        [MemoryOffset(40)] [XdbElement] public FileRef TextureBump;
        [MemoryOffset(48)] [XdbElement] public Float MinScaleFactor;
        [MemoryOffset(52)] [XdbElement] public Float MaxScaleFactor;
        [MemoryOffset(56)] [XdbElement] public Float MinColorFactor;
        [MemoryOffset(60)] [XdbElement] public Float MaxColorFactor;
        [MemoryOffset(64)] [XdbElement] public Float MinBumpFactor;
        [MemoryOffset(68)] [XdbElement] public Float MaxBumpFactor;
        [MemoryOffset(72)] [XdbElement] public Int FadeInTimeMSec;
        [MemoryOffset(76)] [XdbElement] public Int FadeOutTimeMSec;
        [MemoryOffset(80)] [XdbElement("cycleTimeMS")] public Float CycleTimeMs;
    }
}
