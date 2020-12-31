using System.IO;
using DotLiquid;
using DotLiquid.FileSystems;

namespace Bemol.Core {
    public class BemolRenderer {

        public static string Render(string filePath, object model) {
            var root = Path.GetPathRoot(".");
            var folder = "static";

            Template.FileSystem = new LocalFileSystem(folder);
            string text = File.ReadAllTextAsync(folder + filePath).Result;
            Template template = Template.Parse(text);
            return template.Render(Hash.FromAnonymousObject(model));
        }
    }
}
