using System.Linq;
using System.Collections.Generic;

using Bemol.Core;

namespace Bemol.Http {
    internal class HandlerEntry {
        internal readonly HandlerType Type;
        internal readonly string Path;
        internal readonly Handler Handle;
        private readonly PathParser PathParser;

        internal HandlerEntry(HandlerType type, string path, bool ignoreTrailingSlashes, Handler handler) {
            Type = type;
            Path = path;
            Handle = handler;
            PathParser = new PathParser(path, ignoreTrailingSlashes);
        }

        internal bool Matches(string requestUri) {
            return PathParser.Matches(requestUri);
        }

        internal Dictionary<string, string> ExtractPathParams(string requestUri) {
            return PathParser.ExtractPathParams(requestUri);
        }
    }

    internal class PathMatcher {
        private readonly Dictionary<HandlerType, List<HandlerEntry>> HandlerEntries;

        internal PathMatcher() {
            // TODO: There probably exists a more clever way to initialize this dictionary.
            HandlerEntries = new Dictionary<HandlerType, List<HandlerEntry>>();
            foreach (var type in System.Enum.GetValues<HandlerType>()) {
                HandlerEntries.Add(type, new List<HandlerEntry>());
            }
        }

        internal void Add(HandlerEntry entry) {
            HandlerEntries[entry.Type].Add(entry);
        }

        internal List<HandlerEntry> FindEntries(HandlerType type, string requestUri) {
            return HandlerEntries[type].Where(entry => Match(entry, requestUri)).ToList();
        }

        internal bool Match(HandlerEntry entry, string requestPath) {
            if (entry.Path == "*") return true;
            if (entry.Path == requestPath) return true;
            else return entry.Matches(requestPath);
        }
    }
}