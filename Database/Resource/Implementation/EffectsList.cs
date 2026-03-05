using Database.DataType.Implementation;
using Database.Resource.Enum;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class EffectsList : Resource
    {
        [MemoryArrayOffset(24, 52)] [XdbArray] public RandomEffect[] Effects;

        public class RandomEffect : Resource
        {
            [MemoryOffset(4)] [XdbElement] public Int Rate;
            [MemoryOffset(12)] [XdbElement] public AsciiString LocatorName;
            [MemoryOffset(24)] [XdbEnum(typeof(FxLocators))] public Int Locator;
            [MemoryOffset(28)] [XdbElement] public Bool FixPoint;
            [MemoryOffset(32)] [XdbEnum(typeof(ETroopMember))] public Int Member;
            [MemoryOffset(36)] [XdbElement("adddedDelay")] public Int AdddedDelay;
            [MemoryOffset(40)] [XdbElement] public FileRef Effect;
            [MemoryOffset(44)] [XdbElement] public Int FadeInTime;
            [MemoryOffset(48)] [XdbElement] public Int FadeOutTime;
        }
    }
}
