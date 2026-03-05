using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class StaticObject : Resource
    {
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> AiMesh;
        [MemoryOffset(28)] [XdbElement] public Int DismountIndoor;
        [MemoryOffset(32)] [XdbElement] public Int ColoredObject;
        [MemoryOffset(36)] [XdbElement] public Int SourceFileCRC;
        [MemoryOffset(40)] [XdbElement] public FileRef SourceFile;
        [MemoryOffset(48)] [XdbElement] public GenericField<Resource> ExportTemplate;
        [MemoryOffset(52)] [XdbElement] public GenericField<Resource> ObjectTemplate;
        [MemoryOffset(56)] [XdbElement] public GenericField<Resource> Collision;
        [MemoryOffset(60)] [XdbElement] public GenericField<Resource> AiCollision;
        [MemoryOffset(64)] [XdbElement] public GenericField<Resource> Music;
        [MemoryOffset(68)] [XdbElement] public GenericField<Resource> AmbienceSound;
        [MemoryArrayOffset(72, 4)] [XdbArray] public Resource[] Parts;
        [MemoryOffset(84)] [XdbElement] public GenericField<Resource> LosData;
        [MemoryArrayOffset(88, 4)] [XdbArray] public Resource[] AreaMiniMaps;
        [MemoryOffset(100)] [XdbElement] public GenericField<Resource> VisibleZoneIndices;
    }
}
