using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using JetBrains.Annotations;
using NLog;

namespace Database.Serialization.Memory
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MemoryOffsetAttribute : Attribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly int Offset;

        public MemoryOffsetAttribute(int offset)
        {
            Offset = offset;
        }

        [HandleProcessCorruptedStateExceptions]
        public virtual void DeserializeField([NotNull] FieldInfo field, IntPtr memoryAddress, object obj)
        {
            Logger.Debug($"Deserializing {field.Name} at {(memoryAddress + Offset).ToString("x8")}");
            if (typeof(IMemoryDeserializable).IsAssignableFrom(field.FieldType))
            {
                try
                {
                    var fieldValue = (IMemoryDeserializable) Activator.CreateInstance(field.FieldType);
                    fieldValue.Deserialize(memoryAddress + Offset);
                    field.SetValue(obj, fieldValue);
                }
                catch (AccessViolationException)
                {
                    Logger.Warn($"Access violation deserializing field {field.Name} at offset {Offset}");
                }
            }
            else
            {
                throw new Exception("Cannot deserialize field");
            }
        }
    }
}
