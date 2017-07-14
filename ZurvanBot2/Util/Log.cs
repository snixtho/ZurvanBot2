using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace ZurvanBot.Util {
    /// <summary>
    /// Easy logging class. It also outputs a parse-able format which is in:
    /// [TIME_STAMP][LOG_LEVEL](CAT(E)(GORY): My Message
    /// The logging part is also thread-safe.
    /// </summary>
    public class Log {
        public enum Elevation {
            Info = 1,
            Error = 2,
            Warning = 3,
            Debug = 4,
            Verbose = 5
        }

        private static Log _instance;
        private bool _enableConsoleOutput = true;
        private object _sync = new object();

        private Stream _consoleOutput = Console.OpenStandardOutput();

        /// <summary>
        /// List of streams to output log lines to.
        /// </summary>
        public List<Stream> OutputStreams = new List<Stream>();

        /// <summary>
        /// The log level that is required for outputting log lines.
        /// Debug is highest, info is lowest. All logs lower or equal to the
        /// specified log level will be written. Debug log level is default.
        /// </summary>
        public Elevation LogLevel { get; set; }

        /// <summary>
        /// Set to true to prepend a timestamp to the log lines.
        /// Set to false to not have a timestamp. Default is true.
        /// </summary>
        public bool AddTimestamp { get; set; }

        /// <summary>
        /// The format of the timestamp. It uses the format specified here:
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
        /// Default is: dd.MM.yyyy-HH:mm:ss
        /// </summary>
        public string TimestampFormat { get; set; }

        /// <summary>
        /// Whether to enable console output while logging.
        /// </summary>
        public bool EnableConsoleOutput {
            get {
                bool v;
                lock (_sync) {
                    v = _enableConsoleOutput;
                }

                return v;
            }
            set {
                lock (_sync) {
                    _enableConsoleOutput = value;
                    if (value)
                        OutputStreams.Add(_consoleOutput);
                    else
                        OutputStreams.Remove(_consoleOutput);
                }
            }
        }

        private Log() {
            OutputStreams.Add(_consoleOutput);
            LogLevel = Elevation.Debug;
            AddTimestamp = true;
            TimestampFormat = "dd.MM.yyyy-HH:mm:ss";
        }

        public static Log Instance() {
            if (_instance == null)
                _instance = new Log();

            return _instance;
        }

        /// <summary>
        /// Writes a line to the desired output stream.
        /// </summary>
        /// <param name="msg">The string to write.</param>
        private void _logLine(string msg) {
            msg += "\n";
            var b = Encoding.UTF8.GetBytes(msg);

            lock (_sync) {
                foreach (var stream in OutputStreams) {
                    stream.Write(b, 0, b.Length);
                    stream.Flush();
                }
            }
        }

        /// <summary>
        /// Logs a line, formatted with some info like timestamp etc.
        /// </summary>
        /// <param name="msg">The message to format.</param>
        private void _logLineFormatted(string msg) {
            if (AddTimestamp)
                msg = getTimestamp() + msg;
            _logLine(msg);
        }

        /// <summary>
        /// Add a file that will have log lines written to it.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="DirectoryNotFoundException">Occurs when the specified directory of the file does not exist.</exception>
        private void _addFile(string filePath) {
            var file = new FileInfo(filePath);
            if (!file.Directory.Exists)
                throw new DirectoryNotFoundException("The specified directory does not exist.");

            Stream stream;
            if (!file.Exists) {
                stream = file.Create();
            }
            else {
                File.Move(file.FullName, file.FullName + ".archive" + new Random().Next(1000000000));
                stream = file.OpenWrite();
            }

            lock (_sync) {
                OutputStreams.Add(stream);
            }
        }

        /// <summary>
        /// Converts abc.def.ghi etc.. to (Abc)(Def)(Ghi) etc..
        /// </summary>
        /// <param name="cat">The value to convert.</param>
        /// <returns>The converted value.</returns>
        private static string generateCat(string cat) {
            cat = cat.Trim();
            if (cat.Equals("")) return "";
            var p = cat.Split('.');
            var o = "";
            foreach (var part in p) {
                if (part.Length == 0) continue;
                var str = new StringBuilder(part);
                var c = str[0];
                if (c >= 97 && c <= 122)
                    c = (char) (c - 32);
                str[0] = c;
                o += "(" + str + ")";
            }
            return o;
        }

        /// <summary>
        /// Generate a timestamp for the logline.
        /// </summary>
        /// <returns>The timestamp for the log line.</returns>
        private string getTimestamp() {
            return "[" + DateTime.Now.ToString(TimestampFormat) + "]";
        }

        /// <summary>
        /// Log a info message.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="cat">The category of the message.</param>
        public static void Info(string msg, string cat = "") {
            if (_instance == null)
                _instance = new Log();
            if ((int) _instance.LogLevel < (int) Elevation.Info) return;
            _instance._logLineFormatted("[I]" + generateCat(cat) + ": " + msg);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="cat">The category of the message.</param>
        public static void Error(string msg, string cat = "") {
            if (_instance == null)
                _instance = new Log();
            if ((int) _instance.LogLevel < (int) Elevation.Error) return;
            _instance._logLineFormatted("[E]" + generateCat(cat) + ": " + msg);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="cat">The category of the message.</param>
        public static void Warning(string msg, string cat = "") {
            if (_instance == null)
                _instance = new Log();
            if ((int) _instance.LogLevel < (int) Elevation.Warning) return;
            _instance._logLineFormatted("[W]" + generateCat(cat) + ": " + msg);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="cat">The category of the message.</param>
        public static void Debug(string msg, string cat = "") {
            if (_instance == null)
                _instance = new Log();
            if ((int) _instance.LogLevel < (int) Elevation.Debug) return;
            _instance._logLineFormatted("[D]" + generateCat(cat) + ": " + msg);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="cat">The category of the message.</param>
        public static void Verbose(string msg, string cat = "") {
            if (_instance == null)
                _instance = new Log();
            if ((int) _instance.LogLevel < (int) Elevation.Verbose) return;
            _instance._logLineFormatted("[V]" + generateCat(cat) + ": " + msg);
        }

        /// <summary>
        /// Add a file that will have log lines written to it.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        public static void AddLogFile(string filePath) {
            if (_instance == null)
                _instance = new Log();
            _instance._addFile(filePath);
        }
    }
}