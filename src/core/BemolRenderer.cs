using System.IO;
using System.Collections.Generic;

using DotLiquid;
using DotLiquid.FileSystems;


namespace Bemol.Core {
    public class BemolRenderer {
        private readonly BemolConfig Config;
        private readonly Dictionary<int, Template> Cache = new Dictionary<int, Template>();

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
            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string resources = BemolUtil.NormalizePath(Config.ResourcesFolder);
            string partials = BemolUtil.NormalizePath(Config.PartialsFolder);
            char separator = Path.DirectorySeparatorChar;
            Template.FileSystem = new LocalFileSystem($"{currentDirectory}{resources}{separator}{partials}");
        }

        private Template GetTemplate(string filePath) {
            filePath = BemolUtil.NormalizePath(filePath);
            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string resources = BemolUtil.NormalizePath(Config.ResourcesFolder);
            char separator = Path.DirectorySeparatorChar;
            string text = File.ReadAllTextAsync($"{currentDirectory}{resources}{separator}{filePath}").Result;
            int hash = text.GetHashCode();
            if (!Cache.ContainsKey(hash)) {
                Cache[hash] = Template.Parse(text);
            }
            return Cache[hash];
        }
    }
}