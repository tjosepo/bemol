using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bemol.Core {
    internal class PathParser {
        private readonly List<string> PathParamNames;
        private readonly string MatchRegex;
        private readonly string PathParamRegex;

        internal PathParser(string path, bool ignoreTrailingSlash) {
            var segments = path.Split("/")
                    .Where(segment => segment.Length > 0)
                    .Select<string, PathSegment>(segment => {
                        if (segment.StartsWith(":")) return new PathSegment.Parameter(segment.Remove(0, 1));
                        if (segment == "*") return new PathSegment.Wildcard();
                        else return new PathSegment.Normal(segment);
                    }).ToList();


            PathParamNames = segments
                    .OfType<PathSegment.Parameter>()
                    .Select(segment => segment.name).ToList();

            var segmentsRegex = segments.Select(segment => segment.AsRegexString());
            var matchRegexSuffix = (ignoreTrailingSlash) ? "/?" : (path.EndsWith("/")) ? "/" : "";
            MatchRegex = ($"^/{String.Join('/', segmentsRegex)}{matchRegexSuffix}$");

            PathParamRegex = MatchRegex.Replace("[^/]+?", "([^/]+?)");
        }

        internal bool Matches(string url) => Regex.Match(url, MatchRegex).Success;

        internal Dictionary<string, string> ExtractPathParams(string url) {
            return PathParamNames.ToDictionary(
                (key) => key,
                (value) => Regex.Match(url, PathParamRegex).Groups.Values.Last().Value
            );
        }
    }

    internal abstract class PathSegment {
        internal abstract string AsRegexString();

        internal class Normal : PathSegment {
            internal readonly string content;

            internal Normal(string content) => this.content = content;

            override internal string AsRegexString() => content;
        }

        internal class Parameter : PathSegment {
            internal readonly string name;

            internal Parameter(string name) => this.name = name;

            override internal string AsRegexString() => "[^/]+?";     // Accepting everything except slash
        }

        internal class Wildcard : PathSegment {
            override internal string AsRegexString() => ".*?";    // Accept everything
        }
    }
}