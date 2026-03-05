using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    public class SlonRoot : Resource
    {
        [MemoryOffset(24)] [XdbElement] public GenericField<Resource> Cameras;
        [MemoryOffset(28)] [XdbElement] public GenericField<Resource> MusicSettings;
        [MemoryOffset(32)] [XdbElement] public GenericField<Resource> AvatarVisObj;
        [MemoryOffset(36)] [XdbElement] public GenericField<Resource> CreatureVisObj;
        [MemoryOffset(40)] [XdbElement] public GenericField<Resource> DeviceVisObj;
        [MemoryOffset(44)] [XdbElement] public GenericField<Resource> ProjectileVisObj;
        [MemoryOffset(48)] [XdbElement] public GenericField<Resource> VisCharTemplates;
        [MemoryOffset(52)] [XdbElement] public GenericField<Resource> HaloTable;
        [MemoryOffset(56)] [XdbElement] public GenericField<Resource> AstralTransitionFog;
        [MemoryOffset(60)] [XdbElement] public GenericField<Resource> AstralTransitionFogStart;
        [MemoryOffset(64)] [XdbElement] public GenericField<Resource> AstralTransitionFogEnd;
        [MemoryOffset(68)] [XdbElement] public GenericField<Resource> ShadowTexture;
        [MemoryOffset(72)] [XdbElement] public GenericField<Resource> VisualSettings;
        [MemoryOffset(76)] [XdbElement] public GenericField<Resource> ColdBreathFx;
        [MemoryOffset(80)] [XdbElement] public GenericField<Resource> Constants;
        [MemoryOffset(84)] [XdbElement] public GenericField<Resource> ShipCollisionFX;
        [MemoryOffset(88)] [XdbElement] public GenericField<Resource> AbordageBubbleTemplate;
        [MemoryOffset(92)] [XdbElement] public GenericField<Resource> PortraitManagerLight;
        [MemoryOffset(96)] [XdbElement] public GenericField<Resource> SpellResistFx;
        [MemoryOffset(100)] [XdbElement] public Int SpellResistFxForestall;
        [MemoryOffset(104)] [XdbElement] public GenericField<Resource> UnderWaterRays;
        [MemoryOffset(108)] [XdbElement] public GenericField<Resource> BlockParryFx;
        [MemoryOffset(112)] [XdbElement] public Int BlockParryFxForestall;
        [MemoryOffset(116)] [XdbElement] public Float MinSelectionBox;
        [MemoryOffset(120)] [XdbElement] public GenericField<Resource> LootStartAction;
        [MemoryOffset(124)] [XdbElement] public GenericField<Resource> LootEndAction;
        [MemoryOffset(128)] [XdbElement] public GenericField<Resource> LevelChangedScript;
        [MemoryOffset(132)] [XdbElement] public GenericField<Resource> FairyLevelChangedScript;
        [MemoryOffset(136)] [XdbElement] public GenericField<Resource> AstralRoot;
        [MemoryOffset(140)] [XdbElement] public GenericField<Resource> ParrySounds;
        [MemoryOffset(144)] [XdbElement] public Int UnarmedWeaponItemClass;
        [MemoryOffset(148)] [XdbElement] public Int LeftArmController;
        [MemoryOffset(152)] [XdbElement] public Int RightArmController;
        [MemoryOffset(156)] [XdbElement] public Int PropertiesOfCreatureAnimations;
    }
}
