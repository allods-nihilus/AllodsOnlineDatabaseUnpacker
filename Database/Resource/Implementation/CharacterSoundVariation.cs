using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class CharacterSoundVariation : Resource
    {
        [MemoryArrayOffset(24, 16)] [XdbArray] public AnimationSound[] AnimationSounds;
        [MemoryArrayOffset(40, 16)] [XdbArray] public SoundAliasStruct[] AliasSounds;

        public class AnimationSound : Resource
        {
            [MemoryOffset(4)] [XdbElement] public FileRef Animation;
            [MemoryOffset(12)] [XdbElement] public FileRef Sound;
        }

        public class SoundAliasStruct : Resource
        {
            [MemoryOffset(4)] [XdbElement] public FileRef Alias;
            [MemoryOffset(12)] [XdbElement] public FileRef Sound;
        }
    }
}
