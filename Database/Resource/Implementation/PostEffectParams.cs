using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class PostEffectParams : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Float DiffuseFactor;
        [MemoryOffset(28)] [XdbElement] public Float OverlayFactor;
        [MemoryOffset(32)] [XdbElement] public Float BlurAddFactor;
        [MemoryOffset(36)] [XdbElement] public Float BlurRadius;
    }
}
