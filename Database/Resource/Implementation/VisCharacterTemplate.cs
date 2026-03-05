using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class VisCharacterTemplate : Resource
    {
        [MemoryOffset(120)] [XdbElement] public GenericField<Resource> AnimationProperties;
        [MemoryOffset(152)] [XdbElement] public Float UiSelectionScale;
        [MemoryOffset(160)] [XdbElement] public Float MassCoefficient;
        [MemoryOffset(248)] [XdbElement] public Float Radius;
    }
}
