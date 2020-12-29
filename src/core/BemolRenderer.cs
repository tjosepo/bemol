using System.IO;
using DotLiquid;

namespace Bemol.Core {
    public class BemolRenderer {


        public static string Render(string filePath, object model) {

            // Template.FileSystem = new LocalFileSystem("resources");
            string text = File.ReadAllTextAsync("resources" + filePath).Result;
            Template template = Template.Parse(text);
            return template.Render(Hash.FromAnonymousObject(model));
        }
    }
}
