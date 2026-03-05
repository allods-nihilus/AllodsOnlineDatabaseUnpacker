using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class AnimatedLight : Resource
    {
        [MemoryArrayOffset(24, 4)] [XdbArray] public Value[] Values;
        [MemoryOffset(40)] [XdbElement] public Float AnimationSpeed;
        [MemoryOffset(44)] [XdbElement] public Float Fps;

        public class Value : Resource
        {
            [MemoryOffset(0)] [XdbElement] public Int Color;
        }
    }
}
