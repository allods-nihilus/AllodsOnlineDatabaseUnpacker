using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class ShipSteeringParams : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Float MaxPitch;
        [MemoryOffset(28)] [XdbElement] public Float MaxRoll;
        [MemoryOffset(32)] [XdbElement] public Float Agility;
        [MemoryOffset(36)] [XdbElement] public Float PitchMul;
        [MemoryOffset(40)] [XdbElement] public Float RollMul;
        [MemoryOffset(44)] [XdbElement] public Float ShakePeriod;
        [MemoryOffset(48)] [XdbElement] public Float ShakeAmplitude;
    }
}
