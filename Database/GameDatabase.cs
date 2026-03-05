using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using NLog;

namespace Database
{
    public static class GameDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, IntPtr> ReversedIndex;
        private static readonly Dictionary<string, IntPtr> DbidIndex;
        private static readonly Dictionary<string, string> TypeNameIndex;
        private static readonly Dictionary<int, string> VtableTypeMap;
        private static readonly HashSet<string> MissingFiles;
        private static HashSet<string> NotIndexedDependencies;
        private static readonly Dictionary<string, string> ClassToJavaName;
        // Maps (simpleClassName, fieldName) → full Java type name of target
        private static readonly Dictionary<string, Dictionary<string, string>> FieldTargetTypes;

        private static IntPtr databasePtr;
        private static HandleRef databaseHandle;

        static GameDatabase()
        {
            ReversedIndex = new Dictionary<string, IntPtr>();
            DbidIndex = new Dictionary<string, IntPtr>();
            TypeNameIndex = new Dictionary<string, string>();
            VtableTypeMap = new Dictionary<int, string>();
            MissingFiles = new HashSet<string>();
            NotIndexedDependencies = new HashSet<string>();
            ClassToJavaName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            FieldTargetTypes = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        }

        public static void RegisterJavaTypeNames(Dictionary<string, string> mapping)
        {
            foreach (var kvp in mapping)
                ClassToJavaName[kvp.Key] = kvp.Value;
            Logger.Info("Registered {0} class-to-Java-name mappings", ClassToJavaName.Count);
        }

        public static string GetJavaTypeNameForClass(string className)
        {
            if (ClassToJavaName.TryGetValue(className, out var javaName))
                return javaName;
            return null;
        }

        public static void InitDataSystem(string dataPath, string localizationExtension)
        {
            if (Wrapper.InitGameDataSystem(dataPath, localizationExtension, false, false, true, false))
                Logger.Info("Game data system loaded from {0}", dataPath);
            else
                throw new Exception($"Could not load game data system from {dataPath}");
            databasePtr = Wrapper.GetMainDatabase();
            databaseHandle = new HandleRef(new object(), databasePtr);
        }

        public static void Populate(string[] fileNames)
        {
            Logger.Info("Start populating game database with {0} files", fileNames.Length);
            foreach (var file in fileNames)
            {
                if (ReversedIndex.ContainsKey(file) || MissingFiles.Contains(file))
                    continue;
                var dbid = Wrapper.GetDBIDByName(databaseHandle, file);
                if (!Wrapper.DoesObjectExist(databasePtr, dbid))
                {
                    MissingFiles.Add(file);
                }
                else
                {
                    var ptr = Wrapper.GetObject(databasePtr, dbid);
                    ReversedIndex.Add(file, ptr);
                    DbidIndex[file] = dbid;
                    try
                    {
                        var typeName = EditorDatabase.GetClassTypeName(file);
                        if (!string.IsNullOrEmpty(typeName))
                            TypeNameIndex[file] = typeName;
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug("Could not resolve type for {0}: {1}", file, ex.Message);
                    }
                    Logger.Debug("Object {0} added to database", file);
                }
            }

            var missing = MissingFiles.Count;
            Logger.Info("Game database populated with {0} files, {1} files are missing", fileNames.Length - missing, missing);
            BuildVtableMap();
        }

        private static void BuildVtableMap()
        {
            // Build a vtable pointer → class name mapping from files that have (TypeName) in filename
            var classRegex = new System.Text.RegularExpressions.Regex(@"\(([^)]+)\)\.xdb");
            foreach (var kvp in ReversedIndex)
            {
                var match = classRegex.Match(kvp.Key);
                if (!match.Success) continue;
                var className = match.Groups[1].ToString();
                try
                {
                    var vtable = Marshal.ReadInt32(kvp.Value);
                    if (vtable != 0 && !VtableTypeMap.ContainsKey(vtable))
                        VtableTypeMap[vtable] = className;
                }
                catch { }
            }

            // Also learn vtable→type from TypeNameIndex entries (editor database resolved types)
            foreach (var kvp in TypeNameIndex)
            {
                if (!ReversedIndex.TryGetValue(kvp.Key, out var ptr)) continue;
                try
                {
                    var vtable = Marshal.ReadInt32(ptr);
                    if (vtable != 0 && !VtableTypeMap.ContainsKey(vtable))
                    {
                        var parts = kvp.Value.Split('.');
                        var className = parts[parts.Length - 1].Replace("$", "_");
                        VtableTypeMap[vtable] = className;
                    }
                }
                catch { }
            }

            Logger.Info("Built vtable type map with {0} unique types", VtableTypeMap.Count);
        }

        public static bool DoesFileExists(string filename)
        {
            return ReversedIndex.ContainsKey(filename);
        }

        public static IntPtr GetObjectPtr(string filename)
        {
            if (!ReversedIndex.TryGetValue(filename, out var result))
                throw new Exception("Could not find object pointer");
            return result;
        }

        public static void AddNotIndexedDependency(string filename)
        {
            if (!NotIndexedDependencies.Contains(filename))
            {
                NotIndexedDependencies.Add(filename);
            }
        }

        public static string[] GetNotIndexedDependencies()
        {
            return NotIndexedDependencies.ToArray();
        }

        public static void ResetNotIndexedDependencies()
        {
            NotIndexedDependencies.Clear();
        }

        public static string[] GetMissingFiles()
        {
            return MissingFiles.ToArray();
        }

        public static void ResetMissingFiles()
        {
            MissingFiles.Clear();
        }

        public static string GetResolvedClassName(string filename)
        {
            if (TypeNameIndex.TryGetValue(filename, out var javaTypeName))
            {
                var parts = javaTypeName.Split('.');
                var name = parts[parts.Length - 1];
                name = name.Replace("$", "_");
                return name;
            }
            // Try filename-based resolution first (works for files with (TypeName) in name)
            var fileClassName = Utils.GetClassName(filename);
            // If the filename didn't contain (TypeName), the result is just the bare filename
            // which won't match any C# class. Try vtable-based resolution.
            if (!filename.Contains("(") && ReversedIndex.TryGetValue(filename, out var ptr))
            {
                try
                {
                    var vtable = Marshal.ReadInt32(ptr);
                    if (VtableTypeMap.TryGetValue(vtable, out var vtableClassName))
                        return vtableClassName;
                }
                catch { }
            }
            return fileClassName;
        }

        public static Dictionary<string, string> GetTypeNameIndex()
        {
            return TypeNameIndex;
        }

        public static string[] GetAllFiles()
        {
            return ReversedIndex.Keys.ToArray();
        }

        public static int GetResourceId(string filename)
        {
            if (!DbidIndex.TryGetValue(filename, out var dbidPtr))
                return -1;
            try
            {
                var dbidHandle = new HandleRef(new object(), dbidPtr);
                return Wrapper.GetResourceId(databaseHandle, dbidHandle);
            }
            catch
            {
                return -1;
            }
        }

        public static string GetJavaTypeName(string filename)
        {
            if (TypeNameIndex.TryGetValue(filename, out var javaTypeName))
                return javaTypeName;
            return null;
        }

        public static string ResolveJavaTypeFromPtr(IntPtr objectPtr)
        {
            if (objectPtr == IntPtr.Zero) return null;
            try
            {
                var vtable = Marshal.ReadInt32(objectPtr);
                if (VtableTypeMap.TryGetValue(vtable, out var className))
                    return GetJavaTypeNameForClass(className);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Parses annotatedTypes.xml to build field-level target type mapping.
        /// For each type's non-primitive fields, stores the full Java type name.
        /// Used by FileRef to resolve xpointer names when vtable lookup fails.
        /// </summary>
        public static void BuildFieldTargetTypes(string annotatedTypesPath)
        {
            var primitives = new HashSet<string> { "int", "float", "boolean", "long", "double", "short", "byte" };
            var stringTypes = new HashSet<string>
            {
                "java.lang.String",
                "annotations.attributes.ASCIIString",
                "resourceDB.resources.LocalizedString",
                "client.commonDBTypes.TextFileRef"
            };

            var doc = XDocument.Load(annotatedTypesPath);
            var totalFields = 0;
            foreach (var typeElem in doc.Root.Elements("type"))
            {
                var typeName = typeElem.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(typeName)) continue;
                var simpleName = typeName.Split('.').Last().Replace("$", "_");

                var fieldsElem = typeElem.Element("fields");
                if (fieldsElem == null) continue;

                var fieldDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var fieldElem in fieldsElem.Elements("field"))
                {
                    var fieldName = fieldElem.Attribute("name")?.Value;
                    var fieldType = fieldElem.Attribute("type")?.Value;
                    if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(fieldType)) continue;
                    if (primitives.Contains(fieldType)) continue;
                    if (stringTypes.Contains(fieldType)) continue;
                    if (fieldType.StartsWith("[L")) continue; // arrays
                    fieldDict[fieldName] = fieldType;
                }

                if (fieldDict.Count > 0)
                {
                    FieldTargetTypes[simpleName] = fieldDict;
                    totalFields += fieldDict.Count;
                }
            }

            Logger.Info("Built field target type map: {0} types, {1} field mappings", FieldTargetTypes.Count, totalFields);
        }

        /// <summary>
        /// Looks up the expected Java type name for a field on a given parent class.
        /// </summary>
        public static string GetFieldTargetType(string className, string fieldName)
        {
            if (FieldTargetTypes.TryGetValue(className, out var fields))
                if (fields.TryGetValue(fieldName, out var targetType))
                    return targetType;
            return null;
        }

        /// <summary>
        /// Learns a vtable→type mapping at runtime from a known object.
        /// Seeds both VtableTypeMap and ClassToJavaName for future lookups.
        /// </summary>
        public static void LearnVtable(IntPtr objectPtr, string simpleClassName, string fullJavaType)
        {
            if (objectPtr == IntPtr.Zero) return;
            try
            {
                var vtable = Marshal.ReadInt32(objectPtr);
                if (vtable == 0) return;
                if (!VtableTypeMap.ContainsKey(vtable))
                {
                    VtableTypeMap[vtable] = simpleClassName;
                    Logger.Debug("Learned vtable 0x{0:X8} → {1}", vtable, simpleClassName);
                }
                if (!ClassToJavaName.ContainsKey(simpleClassName))
                    ClassToJavaName[simpleClassName] = fullJavaType;
            }
            catch { }
        }
    }
}