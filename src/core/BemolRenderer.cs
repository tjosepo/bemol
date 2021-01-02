using System.IO;
using System.Collections.Generic;
using DotLiquid;
using DotLiquid.FileSystems;
using System.Text.RegularExpressions;


namespace Bemol.Core {
    public class BemolRenderer {
        private BemolConfig Config;
        private Dictionary<int, Template> Cache = new Dictionary<int, Template>();

        public BemolRenderer(BemolConfig config) {
            Config = config;
            SetFileSystem();
        }

        public string Render(string filePath, object model = null) {
            var template = GetTemplate(filePath);
            var hash = Hash.FromAnonymousObject(model);
            return template.Render(hash);
        }

        private void SetFileSystem() {
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var resources = NormalizePaths(Config.ResourcesFolder);
            var partials = NormalizePaths(Config.PartialsFolder);
            var separator = Path.DirectorySeparatorChar;
            Template.FileSystem = new LocalFileSystem($"{currentDirectory}{resources}{separator}{partials}");
        }

        private Template GetTemplate(string filePath) {
            filePath = NormalizePaths(filePath);
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var resources = NormalizePaths(Config.ResourcesFolder);
            var separator = Path.DirectorySeparatorChar;
            var text = File.ReadAllTextAsync($"{currentDirectory}{resources}{separator}{filePath}").Result;
            var hash = text.GetHashCode();
            if (!Cache.ContainsKey(hash)) {
                Cache[hash] = Template.Parse(text);
            }
            return Cache[hash];
        }

        public string NormalizePaths(string path) {
            if (Regex.IsMatch(path, @"^[\/|\\]")) path = path.Remove(0, 1);
            if (Regex.IsMatch(path, @"[\/|\\]$")) path = path.Remove(path.Length - 1, 1);
            var separator = Path.DirectorySeparatorChar;
            path = path.Replace("\\", $"{separator}");
            path = path.Replace("/", $"{separator}");
            return path;
        }
    }
}