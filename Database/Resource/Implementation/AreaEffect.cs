using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class AreaEffect : Resource
    {
        [MemoryOffset(24)] [XdbElement] public Bool RandomRotate;
        [MemoryOffset(28)] [XdbElement] public Float FadeFactor;
        [MemoryOffset(32)] [XdbElement] public Float MaxEffectSpeed;
        [MemoryOffset(36)] [XdbElement] public FileRef Effect;
        [MemoryOffset(44)] [XdbElement] public FileRef AstralParams;
        [MemoryOffset(52)] [XdbElement] public Int AreaEffectGridStep;
    }
}
