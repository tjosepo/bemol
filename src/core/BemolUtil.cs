using System.IO;
using System.Text.RegularExpressions;

namespace Bemol.Core {
    public class BemolUtil {
        /// <summary>
        /// Normalizes paths by removing leading and trailing separators,
        /// and by replacing all other separators with the system's prefered separator.
        /// </summary>
        public static string NormalizePath(string path) {
            if (Regex.IsMatch(path, @"^[\/|\\]")) path = path.Remove(0, 1);
            if (Regex.IsMatch(path, @"[\/|\\]$")) path = path.Remove(path.Length - 1, 1);
            char separator = Path.DirectorySeparatorChar;
            path = path.Replace("\\", $"{separator}");
            path = path.Replace("/", $"{separator}");
            return path;
        }
    }
}