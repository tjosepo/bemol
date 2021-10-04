using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bemol.Routing {
  internal static class PathParser {
    internal static (string?, string?) Cons(string? path) {
      var segments = path?.Split('/')
        .Where(segment => segment.Length > 0);

      var head = segments?.FirstOrDefault();
      var tail = segments?.Skip(1)
        .DefaultIfEmpty<string?>(null)
        .Aggregate((a, b) => a + '/' + b);
      return (head, tail);
    }

    internal static bool Matches(string route, string path, bool ignoreTrailingSlash = true) {
      if (route == "*") return true;
      if (route == path) return true;
      else return SegmentsMatch(route, path, ignoreTrailingSlash);
    }

    private static bool SegmentsMatch(string route, string path, bool ignoreTrailingSlash) {
      var segments = GetSegments(route);
      var pattern = GetPattern(route, segments, ignoreTrailingSlash);
      return Regex.Match(path, pattern).Success;
    }

    private static IEnumerable<PathSegment> GetSegments(string route) {
      return route
        .Split("/")
        .Where(segment => segment.Length > 0)
        .Select<string, PathSegment>(segment => {
          if (segment.StartsWith(":")) return new PathSegment.Parameter(segment.Remove(0, 1));
          if (segment == "*") return new PathSegment.Wildcard();
          else return new PathSegment.Normal(segment);
        })
        .ToList();
    }

    private static string GetPattern(string route, IEnumerable<PathSegment> segments, bool ignoreTrailingSlash) {
      var segmentsRegex = segments.Select(segment => segment.AsRegexString());
      var suffixRegex = (ignoreTrailingSlash) ? "/?" : (route.EndsWith("/")) ? "/" : "";
      return $"^/?{String.Join('/', segmentsRegex)}{suffixRegex}$";
    }

    internal static Dictionary<string, string> ExtractPathParams(string route, string path) {
      var segments = GetSegments(route);
      var pattern = GetPattern(route, segments, true);
      var pathParamPattern = pattern.Replace("[^/]+?", "([^/]+?)");
      var pathParamNames = GetPathParamNames(segments);
      return pathParamNames.ToDictionary(
          (key) => key,
          (value) => Regex.Match(path, pathParamPattern).Groups.Values.Last().Value
      );
    }

    private static List<string> GetPathParamNames(IEnumerable<PathSegment> segments) {
      return segments
        .OfType<PathSegment.Parameter>()
        .Select(segment => segment.name)
        .ToList();
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
}