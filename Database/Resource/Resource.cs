using System;
using System.Reflection;
using System.Xml.Linq;
using Database.DataType.Implementation;
using Database.Serialization.Memory;
using Database.Serialization.XDB;
using NLog;

namespace Database.Resource
{
    public class Resource : IMemoryDeserializable, IXdbSerializable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Deserialize(IntPtr memoryAddress)
        {
            foreach (var field in GetType().GetFields())
                using (var e = field.GetCustomAttributes(typeof(MemoryOffsetAttribute)).GetEnumerator())
                {
                    if (e.MoveNext() && e.Current is MemoryOffsetAttribute offsetAttribute)
                    {
                        try
                        {
                            offsetAttribute.DeserializeField(field, memoryAddress, this);
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug($"Skipping field {field.Name}: {ex.GetType().Name}");
                        }
                    }
                }
        }

        public XElement Serialize(string name)
        {
            var root = new XElement(name);
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var e = field.GetCustomAttributes(typeof(XdbElementAttribute), false).GetEnumerator();
                if (e.MoveNext() && e.Current is XdbElementAttribute xdbElementAttribute)
                {
                    try
                    {
                        FileRef.SetSerializationContext(GetType().Name, field.Name);
                        root.Add(xdbElementAttribute.SerializeField(field, this));
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug($"Skipping serialization of {field.Name}: {ex.GetType().Name}");
                    }
                }
            }

            return root;
        }
    }
}
