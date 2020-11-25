using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bemol.Core {
    public class PathParser {
        private List<string> pathParamNames;
        private string matchRegex;
        private string pathParamRegex;

        public PathParser(string path, bool ignoreTrailingSlash) {
            var segments = path.Split("/")
                    .Where(segment => segment.Length > 0)
                    .Select<string, PathSegment>(segment => {
                        if (segment.StartsWith(":")) return new PathSegment.Parameter(segment.Remove(0, 1));
                        if (segment == "*") return new PathSegment.Wildcard();
                        else return new PathSegment.Normal(segment);
                    }).ToList();


            pathParamNames = segments
                    .OfType<PathSegment.Parameter>()
                    .Select(segment => segment.name).ToList();

            var segmentsRegex = segments.Select(segment => segment.AsRegexString());
            var matchRegexSuffix = (ignoreTrailingSlash) ? "/?" : (path.EndsWith("/")) ? "/" : "";
            matchRegex = ($"^/{String.Join('/', segmentsRegex)}{matchRegexSuffix}$");

            pathParamRegex = matchRegex.Replace("[^/]+?", "([^/]+?)");
        }

        public bool Matches(string url) => Regex.Match(url, matchRegex).Success;

        public Dictionary<string, string> ExtractPathParams(string url) {
            return pathParamNames.ToDictionary(
                (key) => key,
                (value) => Regex.Match(url, pathParamRegex).Groups.Values.Last().Value
            );
        }
    }

    abstract class PathSegment {
        public abstract string AsRegexString();

        public class Normal : PathSegment {
            public string content { set; get; }

            public Normal(string content) => this.content = content;

            override public string AsRegexString() => content;
        }

        public class Parameter : PathSegment {
            public string name { set; get; }

            public Parameter(string name) => this.name = name;

            override public string AsRegexString() => "[^/]+?";     // Accepting everything except slash
        }

        public class Wildcard : PathSegment {
            override public string AsRegexString() => ".*?";    // Accept everything
        }
    }
}