using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class VisEffectBaseList : Resource
    {
        [MemoryArrayOffset(24, 4)] [XdbArray] public Resource[] Effects;
    }
}
