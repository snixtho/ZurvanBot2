using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CSharp;

namespace ZurvanBot.Plugins {
    /// <summary>
    /// Handles run-time compilation of plugin files.
    /// </summary>
    public class PluginDirectory {
        private DirectoryInfo _directory;
        private Assembly _pluginAssembly;

        public CompilerErrorCollection CompilerErrors { get; private set; }

        public string CacheDir => _directory.FullName + "/cache";
        public string MainPluginFile => _directory.FullName + "/Plugin.cs";
        public string AssemblyCacheFile => CacheDir + "/assemblycache.obj";

        public PluginDirectory(DirectoryInfo dir) {
            if (!dir.Exists)
                throw new FileNotFoundException("The specified plugin directory does not exist.");

            if (!File.Exists(dir.FullName + "/" + "Plugin.cs"))
                throw new Exception("The file 'Plugin.cs' could not be found in the plugin directory.");

            _directory = dir;
        }

        /// <summary>
        /// Recursively retrives all c# source files from a root directory.
        /// </summary>
        /// <param name="dir">The root directory.</param>
        /// <returns>All files found with a .cs extension.</returns>
        private Collection<FileInfo> GetAllSourceFiles(DirectoryInfo dir) {
            var collection = new Collection<FileInfo>();

            foreach (var entry in Directory.GetFileSystemEntries(dir.FullName)) {
                var file = new FileInfo(entry);

                if (file.Attributes.HasFlag(FileAttributes.Directory)) {
                    var newcollection = GetAllSourceFiles(new DirectoryInfo(dir.FullName + "/" + file.Name));
                    foreach (var f in newcollection)
                        collection.Add(f);
                }
                else if (file.Extension.Equals(".cs")) {
                    collection.Add(file);
                }
            }

            return collection;
        }

        /// <summary>
        /// Calculate hash checksum from the specified string.
        /// </summary>
        /// <param name="source">The string to calculate the hash.</param>
        /// <returns>The hash computed by the specified string.</returns>
        private static string CalcChecksum(string source) {
            using (var sha = new SHA512Managed()) {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(source));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash) {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Check to see if the current checksum of the source file
        /// matches the old one.
        /// </summary>
        /// <param name="files">A list of files to check for a complete checksum combination.</param>
        /// <returns>True if they match, false if not.</returns>
        public bool ChecksumMatchesOld(Collection<FileInfo> files) {
            foreach (var file in files) {
                var chksumFile = CacheDir + "/" + file.Name + ".checksum";
                if (!File.Exists(chksumFile)) return false;

                var source = File.ReadAllText(file.FullName);
                var oldChecksum = File.ReadAllText(chksumFile);
                var newChecksum = CalcChecksum(source);

                if (!oldChecksum.Equals(newChecksum)) return false;
            }

            return true;
        }

        /// <summary>
        /// Load the source of all the specified files.
        /// </summary>
        /// <param name="files">A list of source files to load.</param>
        /// <returns>A list of sources for each source file.</returns>
        public StringCollection LoadSources(Collection<FileInfo> files) {
            var sources = new StringCollection();
            foreach (var fileInfo in files)
                sources.Add(File.ReadAllText(fileInfo.FullName));
            return sources;
        }

        /// <summary>
        /// Try to compile the plugin and all it's extra
        /// source files.
        /// </summary>
        /// <returns>True if the compilation was successfull, false if not.</returns>
        public bool TryCompile() {
            if (!Directory.Exists(CacheDir))
                Directory.CreateDirectory(CacheDir);

            // check if cache exists, and load that instead if so
            var sourceFiles = GetAllSourceFiles(new DirectoryInfo(_directory.FullName));
            if (File.Exists(AssemblyCacheFile) && ChecksumMatchesOld(sourceFiles)) {
                _pluginAssembly = Assembly.LoadFile(AssemblyCacheFile);
                Console.WriteLine("Loaded " + this + " from cache.");
                return true;
            }

            var srcFilesArray = new string[sourceFiles.Count];
            for (var i = 0; i < sourceFiles.Count; i++)
                srcFilesArray[i] = File.ReadAllText(sourceFiles[i].FullName);

            var codeProvider = new CSharpCodeProvider();
            var compPars = new CompilerParameters {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = Debugger.IsAttached,
                OutputAssembly = AssemblyCacheFile
            };

            compPars.ReferencedAssemblies.Add(Path.GetFileName(Assembly.GetEntryAssembly().Location));

            var res = codeProvider.CompileAssemblyFromSource(compPars, srcFilesArray);

            CompilerErrors = res.Errors;
            if (res.Errors.HasErrors) {
                return false;
            }

            // plugin compilation successful
            _pluginAssembly = res.CompiledAssembly;

            // create checksum file for it
            for (var i = 0; i < srcFilesArray.Length; i++) {
                var code = srcFilesArray[i];
                var fileName = CacheDir + "/" + sourceFiles[i].Name + ".checksum";
                var checksum = CalcChecksum(code);
                File.WriteAllText(fileName, checksum);
            }

            Console.WriteLine("Loaded " + this + " from source.");

            return true;
        }

        /// <summary>
        /// Create an object instance of the compiled plugin class.
        /// </summary>
        /// <param name="pluginSystem"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Plugin CreateInstance(PluginSystem pluginSystem) {
            var pluginClass = _pluginAssembly.GetType("ZurvanBot.Plugins.CustomPlugin");
            if (pluginClass == null)
                throw new Exception(
                    "Failed to find the main plugin class 'CustomPlugin'. Make sure it has public access.");

            var plugin = (Plugin) Activator.CreateInstance(pluginClass, pluginSystem);
            return plugin;
        }

        /// <summary>
        /// Just returns the file name.
        /// </summary>
        /// <returns>The file name.</returns>
        public override string ToString() {
            return _directory.Name;
        }
    }
}