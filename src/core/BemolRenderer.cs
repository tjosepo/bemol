using System.IO;
using System.Collections.Generic;

using DotLiquid;
using DotLiquid.FileSystems;

using Bemol.Routing;

namespace Bemol.Core {
  internal class BemolRenderer {
    private readonly RouterConfig Config;
    private static readonly Dictionary<int, Template> Cache = new Dictionary<int, Template>();

    internal BemolRenderer(RouterConfig config) {
      Config = config;
      SetFileSystem();
    }

    internal string Render(string filePath, object? model) {
      var template = GetTemplate(filePath);
      var hash = Hash.FromAnonymousObject(model);
      return template.Render(hash);
    }

    private void SetFileSystem() {
      char separator = Path.DirectorySeparatorChar;
      string templateFolder = BemolUtil.NormalizePath(Config.TemplateFolder);
      string fullPath = Path.GetFullPath($".{separator}{templateFolder}");
      Template.FileSystem = new LocalFileSystem(fullPath);
    }

    private Template GetTemplate(string filePath) {
      char separator = Path.DirectorySeparatorChar;
      string templateFolder = BemolUtil.NormalizePath(Config.TemplateFolder);
      filePath = BemolUtil.NormalizePath(filePath);
      string fullPath = Path.GetFullPath($".{separator}{templateFolder}{separator}{filePath}");
      string text = File.ReadAllTextAsync(fullPath).Result;
      int hash = text.GetHashCode();
      if (!Cache.ContainsKey(hash)) {
        Cache[hash] = Template.Parse(text);
      }
      return Cache[hash];
    }
  }
}