using Xunit;
using Bemol.Routing;

namespace Bemol.Test {
  public class PathParserTest {

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("/", null, null)]
    [InlineData("/hello", "hello", null)]
    [InlineData("/hello/", "hello", null)]
    [InlineData("hello", "hello", null)]
    [InlineData("/hello/world", "hello", "world")]
    [InlineData("/hello/world/", "hello", "world")]
    [InlineData("///hello//darkness//my//old///friend", "hello", "darkness/my/old/friend")]
    [InlineData("*", "*", null)]
    [InlineData("/*/blog", "*", "blog")]
    [InlineData("/:id/profile", ":id", "profile")]
    public void Cons_ExtractHeadAndTail(string path, string expectedHead, string expectedTail) {
      var (head, tail) = PathParser.Cons(path);

      Assert.Equal(expectedHead, head);
      Assert.Equal(expectedTail, tail);
    }
  }
}