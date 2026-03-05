using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class SkyMesh : Resource
    {
        [MemoryArrayOffset(24, 164)] [XdbArray] public SkyMeshPart[] Parts;
        [MemoryOffset(40)] [XdbElement] public Float NoiseFactor;

        public class SkyMeshPart : Resource
        {
            [MemoryOffset(4)] [XdbElement] public Float WorldCoordX;
            [MemoryOffset(8)] [XdbElement] public Float WorldCoordY;
            [MemoryOffset(12)] [XdbElement] public Float WorldCoordZ;
            [MemoryOffset(16)] [XdbElement] public Bool UseWorldCoord;
            [MemoryOffset(24)] [XdbElement] public Bool Show;
            [MemoryOffset(28)] [XdbElement] public FileRef Animation;
            [MemoryOffset(36)] [XdbElement] public FileRef Geometry;
            [MemoryOffset(44)] [XdbElement] public Float FovFactor;
            [MemoryOffset(48)] [XdbElement] public Float ColorFactor;
            [MemoryOffset(52)] [XdbElement] public Int AmbientColor;
        }
    }
}
