using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;

namespace Database.Resource.Implementation
{
    // Source: gameMechanics.constructor.schemes.item.ContainerShape
    // Base: gameMechanics.constructor.schemes.item.ItemFunctionalPart
    public class ContainerShape : ItemFunctionalPart
    {
        [MemoryArrayOffset(24, 52)] [XdbArray] public Pocket[] Pockets;
        [MemoryOffset(40)] [XdbElement] public Int BaseSize;
    }
}
