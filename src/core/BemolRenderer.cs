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

        public string Render(string filePath, object model) {
            var template = GetTemplate(filePath);
            var hash = Hash.FromAnonymousObject(model);
            return template.Render(hash);
        }

        private void SetFileSystem() {
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var resources = BemolUtil.NormalizePath(Config.ResourcesFolder);
            var partials = BemolUtil.NormalizePath(Config.PartialsFolder);
            var separator = Path.DirectorySeparatorChar;
            Template.FileSystem = new LocalFileSystem($"{currentDirectory}{resources}{separator}{partials}");
        }

        private Template GetTemplate(string filePath) {
            filePath = BemolUtil.NormalizePath(filePath);
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var resources = BemolUtil.NormalizePath(Config.ResourcesFolder);
            var separator = Path.DirectorySeparatorChar;
            var text = File.ReadAllTextAsync($"{currentDirectory}{resources}{separator}{filePath}").Result;
            var hash = text.GetHashCode();
            if (!Cache.ContainsKey(hash)) {
                Cache[hash] = Template.Parse(text);
            }
            return Cache[hash];
        }
    }
}