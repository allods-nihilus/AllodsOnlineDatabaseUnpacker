using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NLog;

namespace Database.Serialization.Memory
{
    public class MemoryArrayOffsetAttribute : MemoryOffsetAttribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int itemSize;

        public MemoryArrayOffsetAttribute(int offset, int itemSize) : base(offset)
        {
            this.itemSize = itemSize;
        }

        public override void DeserializeField(FieldInfo field, IntPtr memoryAddress, object obj)
        {
            Logger.Debug($"Deserializing {field.Name} at {(memoryAddress + Offset).ToString("x8")}");
            var startMemoryAddress = Marshal.ReadIntPtr(memoryAddress + Offset);
            var endMemoryAddress = Marshal.ReadIntPtr(memoryAddress + Offset + 4);

            var elementType = field.FieldType.GetElementType();
            if (elementType is null || !typeof(IMemoryDeserializable).IsAssignableFrom(elementType))
            {
                throw new Exception("Cannot deserialize field");
            }

            if (startMemoryAddress == IntPtr.Zero || endMemoryAddress == IntPtr.Zero || itemSize <= 0)
            {
                field.SetValue(obj, Array.CreateInstance(elementType, 0));
                return;
            }

            var byteLength = endMemoryAddress.ToInt32() - startMemoryAddress.ToInt32();
            if (byteLength < 0 || byteLength > 10 * 1024 * 1024)
            {
                Logger.Warn($"Skipping array {field.Name}: invalid byte range ({byteLength})");
                field.SetValue(obj, Array.CreateInstance(elementType, 0));
                return;
            }

            var count = byteLength / itemSize;
            var array = Array.CreateInstance(elementType, count);
            for (var i = 0; i < count; i++)
            {
                var entry = (IMemoryDeserializable) Activator.CreateInstance(elementType);
                entry.Deserialize(startMemoryAddress + i * itemSize);
                array.SetValue(entry, i);
            }

            field.SetValue(obj, array);
        }
    }
}
