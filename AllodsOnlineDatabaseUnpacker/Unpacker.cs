using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml;
using Database;
using Database.Resource;
using NLog;

#if DEBUG
using System.Threading.Tasks;
#endif

namespace AllodsOnlineDatabaseUnpacker
{
    public class Unpacker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string exportFolder;
        private readonly bool testMode;
        private int successCount;
        private int errorCount;
        private int skippedMissing;
        private int skippedUnimplemented;
        private readonly Dictionary<string, int> unimplementedTypes = new Dictionary<string, int>();

        public Unpacker(bool testMode, string exportFolder)
        {
            this.testMode = testMode;
            this.exportFolder = exportFolder;
        }

        public void Run(string[] objectList)
        {
            Logger.Info($"Starting unpacker with {objectList.Length} files");
            GameDatabase.Populate(objectList);
#if !DEBUG
            foreach (var obj in objectList) BuildObject(obj);
            #else
            Parallel.ForEach(objectList, BuildObject);
#endif
            var notIndexedDependencies = GameDatabase.GetNotIndexedDependencies();
            if (notIndexedDependencies.Length > 0)
            {
                GameDatabase.ResetNotIndexedDependencies();
                // GameDatabase.ResetMissingFiles();
                Run(notIndexedDependencies);
            }
            else
            {
                Logger.Info($"Unpacker finished: {successCount} extracted, {errorCount} errors, {skippedMissing} missing, {skippedUnimplemented} unimplemented");
                if (unimplementedTypes.Count > 0)
                {
                    Logger.Info("Unimplemented types:");
                    foreach (var kv in unimplementedTypes.OrderByDescending(x => x.Value))
                        Logger.Info($"  {kv.Key}: {kv.Value} files");
                }
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void BuildObject(string filePath)
        {
            if (!GameDatabase.DoesFileExists(filePath))
            {
                skippedMissing++;
                return;
            }
            var className = Utils.GetClassName(filePath);
            var type = Type.GetType($"Database.Resource.Implementation.{className}, Database");
            if (type is null)
            {
                skippedUnimplemented++;
                if (!unimplementedTypes.ContainsKey(className))
                    unimplementedTypes[className] = 0;
                unimplementedTypes[className]++;
                return;
            }

            try
            {
                if (!(Activator.CreateInstance(type) is Resource obj))
                {
                    Logger.Error($"Failed to create instance for {filePath}");
                    errorCount++;
                    return;
                }
                obj.Deserialize(GameDatabase.GetObjectPtr(filePath));

                var directoryName = Path.GetDirectoryName(filePath);
                if (directoryName is null)
                {
                    Logger.Error($"Directory name is null for path {filePath}");
                    errorCount++;
                    return;
                }
                var fullDir = Path.GetFullPath(Path.Combine(exportFolder, directoryName));
                var fullFile = Path.GetFullPath(Path.Combine(exportFolder, filePath));
                // Use long path prefix to bypass MAX_PATH limit
                if (!fullDir.StartsWith(@"\\?\")) fullDir = @"\\?\" + fullDir;
                if (!fullFile.StartsWith(@"\\?\")) fullFile = @"\\?\" + fullFile;
                if (!Directory.Exists(fullDir))
                {
                    Directory.CreateDirectory(fullDir);
                }
                using (var writer = new XmlTextWriter(fullFile, new UTF8Encoding(false)))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;
                    if (testMode)
                    {
                        obj.Serialize(className);
                    }
                    else
                    {
                        obj.Serialize(className).Save(writer);
                    }
                }
                successCount++;
            }
            catch (Exception e)
            {
                Logger.Error($"Error processing {filePath}: {e.Message}");
                errorCount++;
            }
        }
    }
}