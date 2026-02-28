using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Database.DataType.Implementation
{
    [UsedImplicitly]
    public class AsciiString : DataType
    {
        private string value;

        public AsciiString(string value)
        {
            this.value = value;
        }

        public AsciiString() { }

        public override XElement Serialize(string name)
        {
            return value == string.Empty ? new XElement(name) : new XElement(name, value);
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            var startAddress = Marshal.ReadIntPtr(memoryAddress);
            var endAddress = Marshal.ReadIntPtr(memoryAddress + 4);

            if (startAddress == IntPtr.Zero || endAddress == IntPtr.Zero)
            {
                value = string.Empty;
                return;
            }

            var length = endAddress.ToInt32() - startAddress.ToInt32();
            if (length <= 0 || length > 4096)
            {
                value = string.Empty;
                return;
            }

            var buffer = new byte[length];
            Marshal.Copy(startAddress, buffer, 0, length);
            var sb = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                if (buffer[i] != 0) sb.Append((char) buffer[i]);
            }

            value = sb.ToString();
        }

        public override string ToString()
        {
            return Utils.NormalizePath(value);
        }
    }
}
