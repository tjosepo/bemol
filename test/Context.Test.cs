using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Xunit;
using Bemol.Http;
using Bemol.Http.Exceptions;
using Bemol.Test.Fixtures;

namespace Bemol.Test {

    [CollectionDefinition("Context")]
    public class ContextServer : ICollectionFixture<BemolServerFixture> { }

    [Collection("Context")]
    public class ContextTest {

        BemolServerFixture Server;

        public ContextTest(BemolServerFixture server) {
            Server = server;
        }

        [Fact]
        public void Body_Equal() {
            HttpContent content = new StringContent("Hello world!");
            Context ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Equal("Hello world!", ctx.Body());
        }

        [Fact]
        public void BodyAsBytes_Equal() {
            HttpContent content = new StringContent("Hello world!");
            Context ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Equal(Encoding.UTF8.GetBytes("Hello world!"), ctx.BodyAsBytes());
        }

        private class User {
            public string Name { set; get; }
            public int Age { set; get; }
            public User(string name = "John", int age = 25) {
                Name = name;
                Age = age;
            }
        }

        [Fact]
        public void BodyAsClass_Equal() {
            var user = new User();
            HttpContent content = JsonContent.Create(user);
            Context ctx = Server.GetContext(client => client.PostAsJsonAsync("/", content));
            var returned = ctx.BodyAsClass<User>();
            Assert.Equal(user.Age, returned.Age);
            Assert.Equal(user.Name, returned.Name);
        }

        [Fact]
        public void BodyAsClass_NotValid() {
            Context ctx = Server.GetContext(client => client.PostAsync("/", null));
            Assert.Throws<BadRequestException>(() => ctx.BodyAsClass<User>());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/user")]
        [InlineData("/user/")]
        public void Path_Equal(string path) {
            Context ctx = Server.GetContext(client => client.GetAsync(path));
            Assert.Equal(path, ctx.Path());
        }

        [Fact]
        public void Method_Get_Equal() {
            Context ctx = Server.GetContext(client => client.GetAsync("/"));
            Assert.Equal("GET", ctx.Method());
        }

        [Fact]
        public void Method_Post_Equal() {
            Context ctx = Server.GetContext(client => client.PostAsync("/", null));
            Assert.Equal("POST", ctx.Method());
        }

        [Theory]
        [InlineData("Hello world!")]
        [InlineData("Tommy Josépovic")]
        [InlineData("На берегу пустынных волн")]
        [InlineData("ვეპხის ტყაოსანი შოთა რუსთაველი")]
        [InlineData("⠊⠀⠉⠁⠝⠀⠑⠁⠞⠀⠛⠇⠁⠎⠎⠀⠁⠝⠙⠀⠊⠞⠀⠙⠕⠑⠎⠝⠞⠀⠓⠥⠗⠞⠀⠍⠑")]
        public void ResultString_SetUsingString_Equal(string result) {
            Context ctx = Server.GetContext(client => client.GetAsync("/"));
            ctx.Result(result);
            Assert.Equal(result, ctx.ResultString());
        }

        [Theory]
        [InlineData("Hello world!")]
        [InlineData("Tommy Josépovic")]
        [InlineData("На берегу пустынных волн")]
        [InlineData("ვეპხის ტყაოსანი შოთა რუსთაველი")]
        [InlineData("⠊⠀⠉⠁⠝⠀⠑⠁⠞⠀⠛⠇⠁⠎⠎⠀⠁⠝⠙⠀⠊⠞⠀⠙⠕⠑⠎⠝⠞⠀⠓⠥⠗⠞⠀⠍⠑")]
        public void ResultString_SetUsingBytes_Equal(string result) {
            Context ctx = Server.GetContext(client => client.GetAsync("/"));
            byte[] byteArray = Encoding.UTF8.GetBytes(result);
            ctx.Result(byteArray);
            Assert.Equal(result, ctx.ResultString());
        }

        [Theory]
        [InlineData(404)]
        [InlineData(500)]
        public void Status_SetUsingStatus_Equal(int status) {
            Context ctx = Server.GetContext(client => client.GetAsync("/"));
            ctx.Status(status);
            Assert.Equal(status, ctx.Status());
        }

        [Fact]
        public void Json_SimpleObject_Equal() {
            Context ctx = Server.GetContext(client => client.GetAsync("/"));
            ctx.Json(new { Name = "John", Age = 10 });
            Assert.Equal("{\"Name\":\"John\",\"Age\":10}", ctx.ResultString());
        }
    }
}