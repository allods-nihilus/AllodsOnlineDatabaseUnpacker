using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class VisualItem : Resource
    {
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> VisualItemClass;
        [MemoryOffset(28)] [XdbElement] public Int DressSlot;
        [MemoryOffset(32)] [XdbElement] public GenericField<Resource> TexturePatches;
        [MemoryArrayOffset(36, 4)] [XdbArray] public Resource[] Objects;
        [MemoryOffset(48)] [XdbElement] public GenericField<Resource> ArmorShapes;
        [MemoryArrayOffset(52, 4)] [XdbArray] public Resource[] DisabledGeosets;
        [MemoryOffset(64)] [XdbElement] public GenericField<Resource> HiddenGeosets;
        [MemoryOffset(68)] [XdbElement] public Int HiddenLocators;
        [MemoryOffset(72)] [XdbElement] public GenericField<Resource> Underwear;
        [MemoryOffset(76)] [XdbElement] public GenericField<Resource> VisItemEffects;
        [MemoryOffset(80)] [XdbElement] public GenericField<Resource> DualWield;
        [MemoryOffset(84)] [XdbElement] public GenericField<Resource> SfxMaterial;
        [MemoryOffset(88)] [XdbElement] public GenericField<Resource> VisualAliases;
        [MemoryOffset(92)] [XdbElement] public GenericField<Resource> Grades;
    }
}
