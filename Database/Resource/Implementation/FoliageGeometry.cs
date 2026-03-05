using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class FoliageGeometry : Resource
    {
        [MemoryOffset(24)] [XdbElement("sourceFileCRC")] public Int SourceFileCrc;
        [MemoryArrayOffset(28, 24)] [XdbArray] public GrassQuad[] Quads;

        public class GrassQuad : Resource
        {
            [MemoryOffset(4)] [XdbElement] public Float U;
            [MemoryOffset(8)] [XdbElement] public Float V;
            [MemoryOffset(12)] [XdbElement] public Float X;
            [MemoryOffset(16)] [XdbElement] public Float Y;
            [MemoryOffset(20)] [XdbElement] public Float Z;
        }
    }
}
