using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Database;
using NLog;

namespace AllodsOnlineDatabaseUnpacker
{
    public class MemoryProbe
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string dataDir;
        private Dictionary<string, TypeDef> typeDefsCache;

        public MemoryProbe(string dataDir)
        {
            this.dataDir = dataDir;
        }

        public class FieldDef
        {
            public string Name;
            public string JavaType;
            public string Visibility;
            public string CSharpType;
            public int EstimatedSize;
            public int Alignment;
        }

        public class TypeDef
        {
            public string FullName;
            public string SimpleName;
            public string BaseType;
            public List<FieldDef> Fields = new List<FieldDef>();
        }

        public class ComparisonResult
        {
            public int ByteCount;
            public int InstanceCount;
            public bool[] IsConstant; // per byte
            public byte[][] Dumps;
        }

        public class OffsetSuggestion
        {
            public string FieldName;
            public string CSharpType;
            public int SuggestedOffset;
            public string Confidence;
            public string Reason;
        }

        /// <summary>
        /// Parse annotatedTypes.xml and cache all type definitions.
        /// </summary>
        public Dictionary<string, TypeDef> LoadAllTypeDefs()
        {
            if (typeDefsCache != null) return typeDefsCache;

            var xmlPath = Path.Combine(dataDir, "Types", "annotatedTypes.xml");
            if (!File.Exists(xmlPath))
                throw new FileNotFoundException($"annotatedTypes.xml not found at {xmlPath}");

            var doc = XDocument.Load(xmlPath);
            typeDefsCache = new Dictionary<string, TypeDef>();

            foreach (var typeElem in doc.Root.Elements("type"))
            {
                var fullName = typeElem.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(fullName)) continue;

                var td = new TypeDef
                {
                    FullName = fullName,
                    SimpleName = GetSimpleName(fullName),
                    BaseType = typeElem.Element("baseType")?.Value ?? ""
                };

                var fieldsElem = typeElem.Element("fields");
                if (fieldsElem != null)
                {
                    foreach (var fieldElem in fieldsElem.Elements("field"))
                    {
                        var fd = new FieldDef
                        {
                            Name = fieldElem.Attribute("name")?.Value ?? "",
                            JavaType = fieldElem.Attribute("type")?.Value ?? "",
                            Visibility = fieldElem.Attribute("visibility")?.Value ?? "both"
                        };
                        ResolveFieldTypeInfo(fd);
                        td.Fields.Add(fd);
                    }
                }

                typeDefsCache[fullName] = td;
            }

            Logger.Info("Loaded {0} type definitions from annotatedTypes.xml", typeDefsCache.Count);
            return typeDefsCache;
        }

        /// <summary>
        /// Get the simple class name from a Java fully qualified name.
        /// Handles $ inner class notation by replacing with _.
        /// </summary>
        private static string GetSimpleName(string fullName)
        {
            var parts = fullName.Split('.');
            return parts[parts.Length - 1].Replace("$", "_");
        }

        /// <summary>
        /// Find the TypeDef for a given type name (simple or full).
        /// </summary>
        public TypeDef FindTypeDef(string typeName)
        {
            var defs = LoadAllTypeDefs();

            // Try exact match first (full name)
            if (defs.TryGetValue(typeName, out var exact))
                return exact;

            // Try exact simple name match
            var matches = defs.Values.Where(t => t.SimpleName == typeName).ToList();
            if (matches.Count == 1) return matches[0];
            if (matches.Count > 1)
            {
                Logger.Warn("Multiple types match '{0}':", typeName);
                foreach (var m in matches)
                    Logger.Warn("  {0}", m.FullName);
                Logger.Warn("Using first match: {0}", matches[0].FullName);
                return matches[0];
            }

            // Try case-insensitive simple name match (handles BLOb vs Blob, etc.)
            matches = defs.Values.Where(t =>
                string.Equals(t.SimpleName, typeName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count >= 1)
            {
                if (matches.Count > 1)
                {
                    Logger.Warn("Multiple types match '{0}' (case-insensitive):", typeName);
                    foreach (var m in matches)
                        Logger.Warn("  {0}", m.FullName);
                }
                return matches[0];
            }

            return null;
        }

        /// <summary>
        /// Get all fields for a type, including inherited ones.
        /// Returns (ownFields, inheritedFields).
        /// </summary>
        public (List<FieldDef> own, List<FieldDef> inherited) GetAllFields(TypeDef td)
        {
            var inherited = new List<FieldDef>();
            var defs = LoadAllTypeDefs();

            var baseTypeName = td.BaseType;
            while (!string.IsNullOrEmpty(baseTypeName))
            {
                if (defs.TryGetValue(baseTypeName, out var baseTd))
                {
                    inherited.InsertRange(0, baseTd.Fields);
                    baseTypeName = baseTd.BaseType;
                }
                else break;
            }

            return (td.Fields, inherited);
        }

        /// <summary>
        /// Resolve Java type string to C# type info (type name, size, alignment).
        /// </summary>
        private void ResolveFieldTypeInfo(FieldDef fd)
        {
            var jt = fd.JavaType;

            if (jt == "int")
            {
                fd.CSharpType = "Int";
                fd.EstimatedSize = 4;
                fd.Alignment = 4;
            }
            else if (jt == "float")
            {
                fd.CSharpType = "Float";
                fd.EstimatedSize = 4;
                fd.Alignment = 4;
            }
            else if (jt == "boolean")
            {
                fd.CSharpType = "Bool";
                fd.EstimatedSize = 1;
                fd.Alignment = 1;
            }
            else if (jt == "java.lang.String" || jt == "ASCIIString" ||
                     jt.EndsWith("ASCIIString"))
            {
                fd.CSharpType = "AsciiString";
                fd.EstimatedSize = 8;
                fd.Alignment = 4;
            }
            else if (jt.StartsWith("[L"))
            {
                // Array type: [Lsome.package.Type;
                var elementType = jt.Substring(2).TrimEnd(';');
                fd.CSharpType = "Array";
                fd.EstimatedSize = 8; // start/end pointer pair
                fd.Alignment = 4;
            }
            else if (jt == "client.commonDBTypes.FileRef" ||
                     jt.EndsWith(".FileRef") ||
                     jt == "resourceDB.types.FileRef")
            {
                fd.CSharpType = "FileRef";
                fd.EstimatedSize = 4;
                fd.Alignment = 4;
            }
            else if (jt == "java.lang.Class")
            {
                // Class reference - treated as a generic pointer field
                fd.CSharpType = "GenericField";
                fd.EstimatedSize = 4;
                fd.Alignment = 4;
            }
            else
            {
                // Could be an enum, a nested resource, or a GenericField
                fd.CSharpType = "GenericField";
                fd.EstimatedSize = 4;
                fd.Alignment = 4;
            }
        }

        /// <summary>
        /// Find files of a given type from the TypeNameIndex, falling back to
        /// resolved class name matching and filename pattern matching.
        /// </summary>
        public string[] FindFilesOfType(string simpleClassName, int maxCount = 5)
        {
            var results = new List<string>();

            // First try TypeNameIndex (Java type name resolution)
            var typeIndex = GameDatabase.GetTypeNameIndex();
            foreach (var kv in typeIndex)
            {
                var parts = kv.Value.Split('.');
                var name = parts[parts.Length - 1].Replace("$", "_");
                if (name == simpleClassName)
                {
                    results.Add(kv.Key);
                    if (results.Count >= maxCount) return results.ToArray();
                }
            }

            // Fallback: search all files by resolved class name (handles case mismatches
            // like BLOb in annotatedTypes.xml vs Blob in filenames)
            if (results.Count == 0)
            {
                foreach (var file in GameDatabase.GetAllFiles())
                {
                    var resolved = GameDatabase.GetResolvedClassName(file);
                    if (string.Equals(resolved, simpleClassName, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(file);
                        if (results.Count >= maxCount) return results.ToArray();
                    }
                }
            }

            // Last resort: search by filename pattern (e.g., "(VisualMob).xdb" or "/VisualMob.xdb")
            if (results.Count == 0)
            {
                var pattern1 = $"({simpleClassName}).xdb";
                var pattern2 = $"/{simpleClassName}.xdb";
                foreach (var file in GameDatabase.GetAllFiles())
                {
                    if (file.EndsWith(pattern1, StringComparison.OrdinalIgnoreCase) ||
                        file.EndsWith(pattern2, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(file);
                        if (results.Count >= maxCount) break;
                    }
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Read N bytes from an object pointer.
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        public byte[] DumpMemory(IntPtr ptr, int byteCount)
        {
            var buffer = new byte[byteCount];
            try
            {
                Marshal.Copy(ptr, buffer, 0, byteCount);
            }
            catch (AccessViolationException)
            {
                Logger.Warn("Access violation reading {0} bytes from 0x{1:X8}", byteCount, ptr.ToInt32());
                // Try reading byte by byte to find readable extent
                for (int i = 0; i < byteCount; i++)
                {
                    try
                    {
                        buffer[i] = Marshal.ReadByte(ptr + i);
                    }
                    catch
                    {
                        Logger.Warn("Readable extent: {0} bytes", i);
                        break;
                    }
                }
            }
            return buffer;
        }

        /// <summary>
        /// Compare multiple memory dumps and mark constant vs variable bytes.
        /// </summary>
        public ComparisonResult CompareInstances(byte[][] dumps)
        {
            if (dumps.Length == 0) return new ComparisonResult { ByteCount = 0, InstanceCount = 0 };

            var byteCount = dumps[0].Length;
            var isConstant = new bool[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                isConstant[i] = true;
                var refByte = dumps[0][i];
                for (int j = 1; j < dumps.Length; j++)
                {
                    if (dumps[j][i] != refByte)
                    {
                        isConstant[i] = false;
                        break;
                    }
                }
            }

            return new ComparisonResult
            {
                ByteCount = byteCount,
                InstanceCount = dumps.Length,
                IsConstant = isConstant,
                Dumps = dumps
            };
        }

        /// <summary>
        /// Estimate the total memory size for a type's fields.
        /// Since we don't know which fields are inline embedded Resources (which can be
        /// very large) vs simple 4-byte pointers, we use a generous estimate.
        /// Each non-primitive field could be an inline Resource of ~80+ bytes.
        /// </summary>
        private int EstimateObjectSize(List<FieldDef> allFields)
        {
            // Start at offset 4 (after vtable pointer)
            int size = 4;
            foreach (var f in allFields)
            {
                // Align
                if (f.Alignment > 1)
                    size = (size + f.Alignment - 1) / f.Alignment * f.Alignment;

                // Non-primitive "GenericField" types might actually be inline embedded
                // Resources that take 40-100+ bytes in memory, not just 4
                if (f.CSharpType == "GenericField")
                    size += 80; // generous estimate for inline resource
                else
                    size += f.EstimatedSize;
            }
            // Add safety margin
            return Math.Max(size * 2, 256);
        }

        /// <summary>
        /// Check if a 4-byte value at offset looks like a valid pointer.
        /// Heuristic: in range 0x00400000-0x7FFFFFFF for 32-bit user space.
        /// </summary>
        private bool LooksLikePointer(byte[] dump, int offset)
        {
            if (offset + 4 > dump.Length) return false;
            var val = BitConverter.ToInt32(dump, offset);
            return val >= 0x00400000 && val <= 0x7FFFFFFF;
        }

        /// <summary>
        /// Check if an 8-byte region looks like an AsciiString (pointer pair or both zero).
        /// </summary>
        private bool LooksLikeStringPair(byte[] dump, int offset)
        {
            if (offset + 8 > dump.Length) return false;
            var start = BitConverter.ToInt32(dump, offset);
            var end = BitConverter.ToInt32(dump, offset + 4);
            if (start == 0 && end == 0) return true;
            if (start >= 0x00400000 && start <= 0x7FFFFFFF &&
                end >= 0x00400000 && end <= 0x7FFFFFFF &&
                end >= start && (end - start) < 4096)
                return true;
            return false;
        }

        /// <summary>
        /// Check if an 8-byte region looks like an array (pointer pair where end >= start).
        /// </summary>
        private bool LooksLikeArrayPair(byte[] dump, int offset)
        {
            if (offset + 8 > dump.Length) return false;
            var start = BitConverter.ToInt32(dump, offset);
            var end = BitConverter.ToInt32(dump, offset + 4);
            if (start == 0 && end == 0) return true;
            return start >= 0x00400000 && start <= 0x7FFFFFFF &&
                   end >= start && (end - start) < 10 * 1024 * 1024;
        }

        /// <summary>
        /// Check if a byte looks like a boolean (0x00 or 0x01) across all instances.
        /// </summary>
        private bool LooksLikeBool(byte[][] dumps, int offset)
        {
            foreach (var dump in dumps)
            {
                if (offset >= dump.Length) return false;
                if (dump[offset] != 0x00 && dump[offset] != 0x01) return false;
            }
            return true;
        }

        /// <summary>
        /// Suggest field-to-offset mappings based on field types and memory patterns.
        /// Uses a simple greedy approach: walk through memory assigning fields.
        /// </summary>
        public List<OffsetSuggestion> SuggestOffsets(List<FieldDef> allFields, ComparisonResult comparison)
        {
            var suggestions = new List<OffsetSuggestion>();

            // Start after vtable pointer (offset 4)
            // Walk variable regions and try to match fields
            int currentOffset = 4;
            var dumps = comparison.Dumps;

            foreach (var field in allFields)
            {
                // Align current offset
                if (field.Alignment > 1)
                    currentOffset = (currentOffset + field.Alignment - 1) / field.Alignment * field.Alignment;

                if (currentOffset + field.EstimatedSize > comparison.ByteCount)
                {
                    suggestions.Add(new OffsetSuggestion
                    {
                        FieldName = field.Name,
                        CSharpType = field.CSharpType,
                        SuggestedOffset = -1,
                        Confidence = "NONE",
                        Reason = "Exceeded dump size"
                    });
                    continue;
                }

                string confidence = "LOW";
                string reason = "Sequential placement";

                switch (field.CSharpType)
                {
                    case "Bool":
                        if (LooksLikeBool(dumps, currentOffset))
                        {
                            confidence = "HIGH";
                            reason = "All instances have 0x00 or 0x01";
                        }
                        break;
                    case "Int":
                        confidence = "MEDIUM";
                        reason = "4-byte aligned int";
                        break;
                    case "Float":
                        confidence = "MEDIUM";
                        reason = "4-byte aligned float";
                        break;
                    case "FileRef":
                    case "GenericField":
                        if (dumps.Length > 0 && LooksLikePointer(dumps[0], currentOffset))
                        {
                            confidence = "HIGH";
                            reason = "Looks like valid pointer";
                        }
                        else if (dumps.Length > 0 && BitConverter.ToInt32(dumps[0], currentOffset) == 0)
                        {
                            confidence = "MEDIUM";
                            reason = "Null pointer (valid for optional refs)";
                        }
                        else
                        {
                            confidence = "LOW";
                            reason = "Does not look like pointer";
                        }
                        break;
                    case "AsciiString":
                        if (dumps.Length > 0 && LooksLikeStringPair(dumps[0], currentOffset))
                        {
                            confidence = "HIGH";
                            reason = "Looks like string pointer pair";
                        }
                        break;
                    case "Array":
                        if (dumps.Length > 0 && LooksLikeArrayPair(dumps[0], currentOffset))
                        {
                            confidence = "HIGH";
                            reason = "Looks like array pointer pair";
                        }
                        break;
                }

                suggestions.Add(new OffsetSuggestion
                {
                    FieldName = field.Name,
                    CSharpType = field.CSharpType,
                    SuggestedOffset = currentOffset,
                    Confidence = confidence,
                    Reason = reason
                });

                currentOffset += field.EstimatedSize;
            }

            return suggestions;
        }

        /// <summary>
        /// Main entry point: probe memory layout for a type.
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        public void Probe(string typeName, int dumpSizeOverride = 0)
        {
            var td = FindTypeDef(typeName);
            if (td == null)
            {
                Logger.Error("Type '{0}' not found in annotatedTypes.xml", typeName);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== Memory Probe: {0} ===", td.SimpleName);
            Console.WriteLine("Full name: {0}", td.FullName);
            if (!string.IsNullOrEmpty(td.BaseType))
                Console.WriteLine("Base type: {0}", td.BaseType);

            var (ownFields, inheritedFields) = GetAllFields(td);
            var allFields = inheritedFields.Concat(ownFields).ToList();
            Console.WriteLine("Fields: {0} own + {1} inherited = {2} total",
                ownFields.Count, inheritedFields.Count, allFields.Count);

            // Find instances
            var files = FindFilesOfType(td.SimpleName);
            if (files.Length == 0)
            {
                Console.WriteLine("WARNING: No files found for type '{0}'", td.SimpleName);
                Console.WriteLine("Showing field definitions only:");
                Console.WriteLine();
                PrintFieldDefs(allFields, inheritedFields.Count);
                return;
            }
            Console.WriteLine("Instances found: {0} files", files.Length);
            foreach (var f in files)
                Console.WriteLine("  {0}", f);

            // Estimate dump size
            int dumpSize = dumpSizeOverride > 0 ? dumpSizeOverride : EstimateObjectSize(allFields);
            // Cap at reasonable max
            dumpSize = Math.Min(dumpSize, 2048);
            Console.WriteLine("Dump size: {0} bytes", dumpSize);

            // Dump memory for each instance
            var dumps = new List<byte[]>();
            var ptrs = new List<IntPtr>();
            foreach (var file in files)
            {
                try
                {
                    var ptr = GameDatabase.GetObjectPtr(file);
                    var dump = DumpMemory(ptr, dumpSize);
                    dumps.Add(dump);
                    ptrs.Add(ptr);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Could not dump {0}: {1}", file, ex.Message);
                }
            }

            if (dumps.Count == 0)
            {
                Console.WriteLine("ERROR: Could not dump any instances");
                return;
            }

            // Compare instances
            var comparison = CompareInstances(dumps.ToArray());

            // Print hex dump
            Console.WriteLine();
            PrintHexDump(comparison, files, ptrs);

            // Suggest offsets
            Console.WriteLine();
            Console.WriteLine("--- Field Definitions ---");
            PrintFieldDefs(allFields, inheritedFields.Count);

            Console.WriteLine();
            Console.WriteLine("--- Suggested Mapping ---");
            var suggestions = SuggestOffsets(allFields, comparison);
            PrintSuggestions(suggestions, inheritedFields.Count);

            // Print C# class skeleton
            Console.WriteLine();
            Console.WriteLine("--- C# Skeleton ---");
            PrintCSharpSkeleton(td, ownFields, suggestions, inheritedFields.Count);
        }

        private void PrintFieldDefs(List<FieldDef> allFields, int inheritedCount)
        {
            for (int i = 0; i < allFields.Count; i++)
            {
                var f = allFields[i];
                var marker = i < inheritedCount ? " (inherited)" : "";
                Console.WriteLine("  [{0}] {1,-30} {2,-15} size={3} align={4}{5}",
                    i, f.Name, f.CSharpType, f.EstimatedSize, f.Alignment, marker);
            }
        }

        private void PrintHexDump(ComparisonResult result, string[] files, List<IntPtr> ptrs)
        {
            Console.WriteLine("Hex dump (first {0} bytes), {1} instances:", result.ByteCount, result.InstanceCount);

            // Header
            var header = new StringBuilder();
            header.Append("Offset  ");
            for (int i = 0; i < Math.Min(result.InstanceCount, 5); i++)
                header.AppendFormat("| Inst {0,-10}", i + 1);
            header.Append("| Const?");
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            // Print addresses
            var addrLine = new StringBuilder("Addr    ");
            for (int i = 0; i < Math.Min(result.InstanceCount, 5); i++)
                addrLine.AppendFormat("| {0,-13}", ptrs[i].ToString("X8"));
            Console.WriteLine(addrLine);
            Console.WriteLine(new string('-', header.Length));

            // Body - print 4 bytes per row
            for (int offset = 0; offset < result.ByteCount; offset += 4)
            {
                var line = new StringBuilder();
                line.AppendFormat("0x{0:X4}  ", offset);

                bool rowConstant = true;
                for (int inst = 0; inst < Math.Min(result.InstanceCount, 5); inst++)
                {
                    line.Append("| ");
                    for (int b = 0; b < 4 && (offset + b) < result.ByteCount; b++)
                    {
                        line.AppendFormat("{0:X2} ", result.Dumps[inst][offset + b]);
                        if (!result.IsConstant[offset + b]) rowConstant = false;
                    }
                    // Pad if needed
                    var bytesShown = Math.Min(4, result.ByteCount - offset);
                    for (int b = bytesShown; b < 4; b++)
                        line.Append("   ");
                    line.Append(" ");
                }

                line.AppendFormat("| {0}", rowConstant ? "YES" : "NO");

                // Add int32 interpretation for variable regions
                if (!rowConstant && offset + 4 <= result.ByteCount)
                {
                    var val = BitConverter.ToInt32(result.Dumps[0], offset);
                    line.AppendFormat("  (int={0}, float={1:F3})",
                        val, BitConverter.ToSingle(result.Dumps[0], offset));
                    if (LooksLikePointer(result.Dumps[0], offset))
                        line.Append(" [PTR?]");
                }

                Console.WriteLine(line);
            }
        }

        private void PrintSuggestions(List<OffsetSuggestion> suggestions, int inheritedCount)
        {
            for (int i = 0; i < suggestions.Count; i++)
            {
                var s = suggestions[i];
                var marker = i < inheritedCount ? " (inherited)" : "";
                if (s.SuggestedOffset < 0)
                {
                    Console.WriteLine("  {0,-30} {1,-15} offset=??? confidence={2} -- {3}{4}",
                        s.FieldName, s.CSharpType, s.Confidence, s.Reason, marker);
                }
                else
                {
                    Console.WriteLine("  [MemoryOffset({0,3})] {1,-15} {2,-25} confidence={3} -- {4}{5}",
                        s.SuggestedOffset, s.CSharpType, s.FieldName, s.Confidence, s.Reason, marker);
                }
            }
        }

        private void PrintCSharpSkeleton(TypeDef td, List<FieldDef> ownFields,
            List<OffsetSuggestion> suggestions, int inheritedCount)
        {
            Console.WriteLine("// Auto-generated skeleton for {0}", td.SimpleName);
            Console.WriteLine("// Review offsets carefully before using!");
            Console.WriteLine("public class {0} : Resource", td.SimpleName);
            Console.WriteLine("{{");

            // Only print own fields (skip inherited)
            for (int i = inheritedCount; i < suggestions.Count; i++)
            {
                var s = suggestions[i];
                if (s.SuggestedOffset < 0) continue;

                var fieldName = CapitalizeFirst(s.FieldName);

                switch (s.CSharpType)
                {
                    case "Bool":
                        Console.WriteLine("    [MemoryOffset({0})] [XdbElement] public Bool {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                    case "Int":
                        Console.WriteLine("    [MemoryOffset({0})] [XdbElement] public Int {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                    case "Float":
                        Console.WriteLine("    [MemoryOffset({0})] [XdbElement] public Float {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                    case "FileRef":
                        Console.WriteLine("    [MemoryOffset({0})] [XdbElement] public FileRef {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                    case "AsciiString":
                        Console.WriteLine("    [MemoryOffset({0})] [XdbElement] public AsciiString {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                    case "Array":
                        Console.WriteLine("    // TODO: determine item size for array");
                        Console.WriteLine("    [MemoryArrayOffset({0}, ???)] [XdbArray] public Resource[] {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                    case "GenericField":
                        Console.WriteLine("    [MemoryOffset({0})] [XdbElement] public GenericField<Resource> {1};",
                            s.SuggestedOffset, fieldName);
                        break;
                }
            }

            Console.WriteLine("}}");
        }

        private static string CapitalizeFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <summary>
        /// List all types from annotatedTypes.xml that are not yet implemented as C# classes.
        /// Groups by field count.
        /// </summary>
        public void ListUnimplementedFromXml()
        {
            var defs = LoadAllTypeDefs();
            var implemented = new HashSet<string>();

            // Check which simple names have C# implementations
            foreach (var td in defs.Values)
            {
                var type = Type.GetType($"Database.Resource.Implementation.{td.SimpleName}, Database");
                if (type != null) implemented.Add(td.SimpleName);
            }

            var unimplemented = defs.Values
                .Where(td => !implemented.Contains(td.SimpleName))
                .GroupBy(td => td.Fields.Count)
                .OrderBy(g => g.Key)
                .ToList();

            Console.WriteLine("=== Unimplemented Types from annotatedTypes.xml ===");
            Console.WriteLine("Total types: {0}, Implemented: {1}, Remaining: {2}",
                defs.Count, implemented.Count, defs.Count - implemented.Count);
            Console.WriteLine();

            foreach (var group in unimplemented)
            {
                Console.WriteLine("--- {0} fields ({1} types) ---", group.Key, group.Count());
                foreach (var td in group.OrderBy(t => t.SimpleName))
                {
                    Console.WriteLine("  {0} ({1})", td.SimpleName, td.FullName);
                }
            }
        }
    }
}
