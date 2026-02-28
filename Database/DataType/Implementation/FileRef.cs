using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Database.DataType.Implementation
{
    [UsedImplicitly]
    public class FileRef : DataType
    {
        private IntPtr href;

        public override XElement Serialize(string name)
        {
            if (href == IntPtr.Zero) return new XElement(name, new XAttribute("href", ""));
            var cursor = Marshal.ReadIntPtr(href + 12);
            if (cursor == IntPtr.Zero) return new XElement(name, new XAttribute("href", ""));
            var sb = new StringBuilder();
            for (var i = 0; i < 4096; i++)
            {
                var readByte = Marshal.ReadByte(cursor);
                if (readByte == 0) break;
                sb.Append(Convert.ToChar(readByte));
                cursor += 1;
            }

            var fileName = sb.ToString();
            if (string.IsNullOrEmpty(fileName))
                return new XElement(name, new XAttribute("href", ""));

            var className = Utils.GetClassName(fileName);

            if (!GameDatabase.DoesFileExists(fileName))
            {
                Logger.Warn($"{fileName} is not indexed, it will be processed in next batch");
                GameDatabase.AddNotIndexedDependency(fileName);
            }

            return new XElement(name, new XAttribute("href", $"/{fileName}#xpointer(/{className})"));
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            href = Marshal.ReadIntPtr(memoryAddress);
        }
    }
}
