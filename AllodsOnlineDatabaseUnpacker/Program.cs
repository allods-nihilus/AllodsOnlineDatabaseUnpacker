using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Xml.Linq;
using Database;
using Database.Resource.Implementation;
using Microsoft.Extensions.CommandLineUtils;
using NLog;

namespace AllodsOnlineDatabaseUnpacker
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HandleProcessCorruptedStateExceptions]
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "AlldosDatabaseUnpacker.exe";
            app.Description = "Allods pack.bin database unpacker";

            app.HelpOption("-?|-h|--help");

            var dataDirArgument = app.Argument("data", "Allods data directory (default: data)");
            var exportFolderOption = app.Option("-f|--folder", "Export folder (default: export)", CommandOptionType.SingleValue);
            var devModeOption = app.Option("-d|--dev", "Enable dev mode for memory exploration (default: false)", CommandOptionType.NoValue);
            var testModeOption = app.Option("-t|--test", "Run unpacker without exporting files (default: false)", CommandOptionType.NoValue);
            var indexModeOption = app.Option("-i|--index", "Export editor index to file", CommandOptionType.SingleValue);
            var missingExportModeOption = app.Option("-m|--missing", "Export missing files list to file", CommandOptionType.SingleValue);
            var filesList = app.Option("-l|--list", "Files list to extract", CommandOptionType.SingleValue);
            var classMapOption = app.Option("-c|--class-map", "Export file-to-type class map as TSV", CommandOptionType.SingleValue);
            var unimplementedReportOption = app.Option("-u|--unimplemented-report", "Export unimplemented type names with file counts", CommandOptionType.SingleValue);
            var fullReportOption = app.Option("-r|--full-report", "Export full report TSV (filePath, typeName, status)", CommandOptionType.SingleValue);
            var probeOption = app.Option("-p|--probe", "Probe memory layout for a type (e.g., VisualItem)", CommandOptionType.SingleValue);
            var probeSizeOption = app.Option("--probe-size", "Override dump size in bytes for --probe (default: auto)", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var exportFolder = exportFolderOption.HasValue() ? exportFolderOption.Value() : "export";
                var devMode = devModeOption.HasValue();
                var testMode = testModeOption.HasValue();
                var dataDir = dataDirArgument.Value ?? "data";
                var indexFile = indexModeOption.HasValue() ? indexModeOption.Value() : null;
                var missingFilesFile = missingExportModeOption.HasValue() ? missingExportModeOption.Value() : null;

                Logger.Info("Initializing editor data system");
                EditorDatabase.InitDataSystem(dataDir, "");
                EditorDatabase.Populate();

                Logger.Info("Initializing game data system");
                GameDatabase.InitDataSystem(dataDir, "");

                // Build class-to-Java-name mapping from annotatedTypes.xml
                var annotatedTypesPath = Path.Combine(dataDir, "Types", "annotatedTypes.xml");
                if (File.Exists(annotatedTypesPath))
                {
                    var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    var doc = XDocument.Load(annotatedTypesPath);
                    foreach (var typeElem in doc.Root.Elements("type"))
                    {
                        var fullName = typeElem.Attribute("name")?.Value;
                        if (string.IsNullOrEmpty(fullName)) continue;
                        // Skip client.Replica.* types — they shadow the real types
                        if (fullName.StartsWith("client.Replica.")) continue;
                        var parts = fullName.Split('.');
                        var simpleName = parts[parts.Length - 1].Replace("$", "_");
                        // First mapping wins (don't overwrite)
                        if (!mapping.ContainsKey(simpleName))
                            mapping[simpleName] = fullName;
                    }
                    GameDatabase.RegisterJavaTypeNames(mapping);
                    GameDatabase.BuildFieldTargetTypes(annotatedTypesPath);
                }

                var objectList = EditorDatabase.GetObjectList();

                if (!(indexFile is null))
                {
                    File.WriteAllLines(indexFile, objectList);
                }

                // Pre-populate the game database to build type index
                if (!devMode)
                {
                    GameDatabase.Populate(objectList);
                }

                if (probeOption.HasValue())
                {
                    var probe = new MemoryProbe(dataDir);
                    var probeTypeName = probeOption.Value();
                    int probeSize = probeSizeOption.HasValue() ? int.Parse(probeSizeOption.Value()) : 0;
                    probe.Probe(probeTypeName, probeSize);
                    return 0;
                }

                if (classMapOption.HasValue())
                {
                    var classMapFile = classMapOption.Value();
                    Logger.Info("Exporting class map to {0}", classMapFile);
                    var lines = new List<string> { "filePath\tjavaTypeName\tsimpleClassName" };
                    foreach (var file in objectList)
                    {
                        try
                        {
                            var javaName = EditorDatabase.GetClassTypeName(file);
                            var simpleName = "";
                            if (!string.IsNullOrEmpty(javaName))
                            {
                                var parts = javaName.Split('.');
                                simpleName = parts[parts.Length - 1].Replace("$", "_");
                            }
                            lines.Add($"{file}\t{javaName}\t{simpleName}");
                        }
                        catch (Exception ex)
                        {
                            lines.Add($"{file}\t\t# error: {ex.Message}");
                        }
                    }
                    File.WriteAllLines(classMapFile, lines);
                    Logger.Info("Class map exported: {0} entries", lines.Count - 1);
                }

                if (unimplementedReportOption.HasValue())
                {
                    Logger.Info("Analyzing unimplemented types...");
                    var typeCounts = new Dictionary<string, int>();
                    foreach (var file in objectList)
                    {
                        if (!GameDatabase.DoesFileExists(file)) continue;
                        var className = GameDatabase.GetResolvedClassName(file);
                        var type = Type.GetType($"Database.Resource.Implementation.{className}, Database");
                        if (type is null)
                        {
                            if (!typeCounts.ContainsKey(className))
                                typeCounts[className] = 0;
                            typeCounts[className]++;
                        }
                    }

                    var sorted = typeCounts.OrderByDescending(x => x.Value).ToList();
                    Logger.Info("Found {0} unique unimplemented types ({1} total files)",
                        sorted.Count, sorted.Sum(x => x.Value));

                    var reportFile = unimplementedReportOption.Value();
                    var lines = sorted.Select(kv => $"{kv.Key}\t{kv.Value}").ToList();
                    lines.Insert(0, "typeName\tfileCount");
                    File.WriteAllLines(reportFile, lines);
                    Logger.Info("Unimplemented report exported to {0}", reportFile);
                }

                if (fullReportOption.HasValue())
                {
                    var reportFile = fullReportOption.Value();
                    Logger.Info("Generating full report to {0}", reportFile);
                    var lines = new List<string> { "filePath\ttypeName\tstatus" };
                    foreach (var file in objectList)
                    {
                        var className = GameDatabase.GetResolvedClassName(file);
                        string status;
                        if (!GameDatabase.DoesFileExists(file))
                            status = "missing";
                        else if (Type.GetType($"Database.Resource.Implementation.{className}, Database") is null)
                            status = "unimplemented";
                        else
                            status = "extractable";
                        lines.Add($"{file}\t{className}\t{status}");
                    }
                    File.WriteAllLines(reportFile, lines);
                    Logger.Info("Full report exported: {0} entries", lines.Count - 1);
                }

                if (devMode)
                {
                    string cmd;
                    while ((cmd = Console.ReadLine()) != "exit")
                    {
                        try
                        {
                            var ptr = GameDatabase.GetObjectPtr(cmd);
                            Logger.Info(ptr.ToString("x8"));
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message);
                        }
                    }
                }
                else
                {
                    var unpacker = new Unpacker(testMode, exportFolder);
                    objectList = filesList.HasValue() ? File.ReadAllLines(filesList.Value()) : objectList;

                    unpacker.Run(objectList);
                    if (!(missingFilesFile is null))
                    {
                        var missingList = GameDatabase.GetMissingFiles();
                        File.WriteAllLines(missingFilesFile, missingList);
                    }
                }

                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
            finally
            {
                Console.WriteLine("Program finished, press any key to exit ...");
                if (Console.IsInputRedirected)
                    Environment.Exit(0);
                else
                    Console.ReadKey();
            }
        }
    }
}