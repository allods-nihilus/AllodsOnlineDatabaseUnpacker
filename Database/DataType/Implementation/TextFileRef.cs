using System;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Database.DataType.Implementation
{
    [UsedImplicitly]
    public class TextFileRef : DataType
    {
        private readonly AsciiString href;

        // Thread-static context: directory of the current .xdb file being serialized
        [ThreadStatic] private static string _currentFileDir;

        public static void SetCurrentFileDir(string dir)
        {
            _currentFileDir = dir;
        }

        public TextFileRef()
        {
            href = new AsciiString();
        }

        public override XElement Serialize(string name)
        {
            var path = href.ToString();
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(_currentFileDir))
                path = MakeRelativePath(_currentFileDir, path);
            return new XElement(name, new XAttribute("href", path ?? ""));
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            href.Deserialize(memoryAddress);
        }

        private static string MakeRelativePath(string baseDir, string targetPath)
        {
            var baseParts = baseDir.Replace('\\', '/').Trim('/').Split('/');
            var targetParts = targetPath.Replace('\\', '/').Trim('/').Split('/');

            int common = 0;
            while (common < baseParts.Length && common < targetParts.Length
                   && string.Equals(baseParts[common], targetParts[common], StringComparison.OrdinalIgnoreCase))
                common++;

            var sb = new StringBuilder();
            for (int i = common; i < baseParts.Length; i++)
                sb.Append("../");
            for (int i = common; i < targetParts.Length; i++)
            {
                if (i > common) sb.Append('/');
                sb.Append(targetParts[i]);
            }

            return sb.ToString();
        }
    }
}
