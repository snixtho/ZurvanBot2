using System;

namespace ZurvanBot.Util {
    public class PlatformDetect {
        /// <summary>
        /// Get the string of the current platform.
        /// </summary>
        /// <returns>The string name of the current platform.</returns>
        public static string GetPlatformName() {
            return Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform);
        }

        /// <summary>
        /// Get the current platform.
        /// </summary>
        /// <returns>The current platform.</returns>
        public static PlatformID GetPlatform() {
            return Environment.OSVersion.Platform;
        }

        /// <summary>
        /// Get a more broad platform name, like linux, windows mac etc.
        /// </summary>
        /// <returns></returns>
        public static string GetBroadPlatformName() {
            var platform = GetPlatform();
            if (platform == PlatformID.Win32NT
                || platform == PlatformID.Win32S
                || platform == PlatformID.Win32Windows
                || platform == PlatformID.WinCE) {
                return "windows";
            }
            else if (platform == PlatformID.MacOSX) {
                return "mac";
            }
            else if (platform == PlatformID.Xbox) {
                return "xbox";
            }
            else if (platform == PlatformID.Unix) {
                return "linux";
            }

            return "unknown";
        }
    }
}