using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Database.DataType.Implementation
{
    [UsedImplicitly]
    public class Bool : DataType
    {
        private bool value;

        public override XElement Serialize(string name)
        {
            return new XElement(name, value);
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            var result = Marshal.ReadByte(memoryAddress);
            value = result != 0;
        }
    }
}