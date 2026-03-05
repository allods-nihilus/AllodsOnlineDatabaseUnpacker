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

        // Thread-static context set by Resource.Serialize before each field
        [ThreadStatic] private static string _contextClassName;
        [ThreadStatic] private static string _contextFieldName;

        public static void SetSerializationContext(string className, string fieldName)
        {
            _contextClassName = className;
            _contextFieldName = fieldName;
        }

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

            // Resolve xpointer name:
            // - Files with (TypeName) in name: use short TypeName from filename
            // - Files without: resolve via TypeNameIndex → vtable → field target type → fallback
            string xpointerName;
            if (fileName.Contains("("))
            {
                xpointerName = className;
            }
            else
            {
                xpointerName = GameDatabase.GetJavaTypeName(fileName)
                            ?? GameDatabase.ResolveJavaTypeFromPtr(href);

                // If still unresolved, use field-level type info from annotatedTypes.xml
                if (xpointerName == null && _contextClassName != null && _contextFieldName != null)
                {
                    var targetType = GameDatabase.GetFieldTargetType(_contextClassName, _contextFieldName);
                    if (targetType != null)
                    {
                        xpointerName = targetType;
                        // Learn this vtable→type mapping for future lookups
                        var targetSimple = targetType.Split('.')[targetType.Split('.').Length - 1].Replace("$", "_");
                        GameDatabase.LearnVtable(href, targetSimple, targetType);
                    }
                }

                xpointerName = xpointerName ?? className;
            }

            return new XElement(name, new XAttribute("href", $"/{fileName}#xpointer(/{xpointerName})"));
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            href = Marshal.ReadIntPtr(memoryAddress);
        }
    }
}
