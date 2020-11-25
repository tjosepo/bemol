using Bemol.Core;
using System.Linq;
using System.Collections.Generic;

namespace Bemol.Http {
    public class HandlerEntry {
        public HandlerType type { set; get; }
        public string path { set; get; }
        public Handler handler { set; get; }
        private PathParser pathParser;

        public HandlerEntry(HandlerType type, string path, bool ignoreTrailingSlashes, Handler handler) {
            this.type = type;
            this.path = path;
            this.handler = handler;
            pathParser = new PathParser(path, ignoreTrailingSlashes);
        }

        public bool Matches(string requestUri) {
            return pathParser.Matches(requestUri);
        }

        public Dictionary<string, string> ExtractPathParams(string requestUri) {
            return pathParser.ExtractPathParams(requestUri);
        }
    }

    public class PathMatcher {
        private Dictionary<HandlerType, List<HandlerEntry>> handlerEntries;

        public PathMatcher() {
            // TODO: There probably exists a more clever way to initialize this dictionary.
            handlerEntries = new Dictionary<HandlerType, List<HandlerEntry>>();
            foreach (var type in System.Enum.GetValues<HandlerType>()) {
                handlerEntries.Add(type, new List<HandlerEntry>());
            }
        }

        public void Add(HandlerEntry entry) {
            handlerEntries[entry.type].Add(entry);
        }

        public List<HandlerEntry> FindEntries(HandlerType type, string requestUri) {
            return handlerEntries[type].Where(entry => Match(entry, requestUri)).ToList();
        }

        private bool Match(HandlerEntry entry, string requestPath) {
            if (entry.path == "*") return true;
            if (entry.path == requestPath) return true;
            else return entry.Matches(requestPath);
        }

    }
}