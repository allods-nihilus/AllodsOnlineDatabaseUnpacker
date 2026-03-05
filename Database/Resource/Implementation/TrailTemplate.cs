using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class TrailTemplate : Resource
    {
        [MemoryOffset(24)] [XdbElement] public FileRef TrailTexture;
        [MemoryOffset(32)] [XdbElement] public Int TexCoordsPeriodMs;
        [MemoryOffset(36)] [XdbElement] public Float SpeedStart;
        [MemoryOffset(40)] [XdbElement] public Float SpeedEnd;
        [MemoryOffset(44)] [XdbElement] public Float StretchSpeed;
        [MemoryOffset(48)] [XdbElement] public Int DecayTime;
        [MemoryOffset(52)] [XdbElement] public Int FadeInTimeMSec;
        [MemoryOffset(56)] [XdbElement] public Int FadeOutTimeMSec;
        [MemoryOffset(60)] [XdbElement] public Int Color;
    }
}
