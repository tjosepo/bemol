using System.Linq;
using System.Collections.Generic;

using Bemol.Core;

namespace Bemol.Http {
    internal class HandlerEntry {
        internal readonly string Method;
        internal readonly string Path;
        internal readonly Handler Handle;
        private readonly PathParser PathParser;

        internal HandlerEntry(string method, string path, bool ignoreTrailingSlashes, Handler handler) {
            Method = method;
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
        private readonly Dictionary<string, List<HandlerEntry>> HandlerEntries = new Dictionary<string, List<HandlerEntry>>();

        internal void Add(HandlerEntry entry) {
            if (!HandlerEntries.ContainsKey(entry.Method)) HandlerEntries.Add(entry.Method, new List<HandlerEntry>());
            HandlerEntries[entry.Method].Add(entry);
        }

        internal List<HandlerEntry>? FindEntries(string method, string requestUri) {
            return HandlerEntries.ContainsKey(method)
                ? HandlerEntries[method]?.Where(entry => Match(entry, requestUri)).ToList()
                : null;
        }

        internal bool Match(HandlerEntry entry, string requestPath) {
            if (entry.Path == "*") return true;
            if (entry.Path == requestPath) return true;
            else return entry.Matches(requestPath);
        }
    }
}