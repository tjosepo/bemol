using System.IO;
using System.Text.RegularExpressions;

namespace Bemol.Core {
    public class BemolUtil {
        public static string NormalizePath(string path) {
            if (Regex.IsMatch(path, @"^[\/|\\]")) path = path.Remove(0, 1);
            if (Regex.IsMatch(path, @"[\/|\\]$")) path = path.Remove(path.Length - 1, 1);
            var separator = Path.DirectorySeparatorChar;
            path = path.Replace("\\", $"{separator}");
            path = path.Replace("/", $"{separator}");
            return path;
        }
    }
}