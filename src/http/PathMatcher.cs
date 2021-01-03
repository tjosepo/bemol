using System.Linq;
using System.Collections.Generic;
using Bemol.Core;

namespace Bemol.Http {
    public class HandlerEntry {
        public HandlerType Type { set; get; }
        public string Path { set; get; }
        public Handler Handle { set; get; }
        private PathParser PathParser;

        public HandlerEntry(HandlerType type, string path, bool ignoreTrailingSlashes, Handler handler) {
            Type = type;
            Path = path;
            Handle = handler;
            PathParser = new PathParser(path, ignoreTrailingSlashes);
        }

        public bool Matches(string requestUri) {
            return PathParser.Matches(requestUri);
        }

        public Dictionary<string, string> ExtractPathParams(string requestUri) {
            return PathParser.ExtractPathParams(requestUri);
        }
    }

    public class PathMatcher {
        private Dictionary<HandlerType, List<HandlerEntry>> HandlerEntries;

        public PathMatcher() {
            // TODO: There probably exists a more clever way to initialize this dictionary.
            HandlerEntries = new Dictionary<HandlerType, List<HandlerEntry>>();
            foreach (var type in System.Enum.GetValues<HandlerType>()) {
                HandlerEntries.Add(type, new List<HandlerEntry>());
            }
        }

        public void Add(HandlerEntry entry) {
            HandlerEntries[entry.Type].Add(entry);
        }

        public List<HandlerEntry> FindEntries(HandlerType type, string requestUri) {
            return HandlerEntries[type].Where(entry => Match(entry, requestUri)).ToList();
        }

        private bool Match(HandlerEntry entry, string requestPath) {
            if (entry.Path == "*") return true;
            if (entry.Path == requestPath) return true;
            else return entry.Matches(requestPath);
        }
    }
}