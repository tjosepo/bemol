using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;

using Xunit;
using Bemol.Test.Fixtures;
using Bemol.Http.Exceptions;

namespace Bemol.Test {

    [CollectionDefinition("Context")]
    public class ContextServer : ICollectionFixture<BemolServerFixture> { }

    [Collection("Context")]
    public class ContextTest {

        private readonly BemolServerFixture Server;

        private class User {
            public string Name { set; get; }
            public int Age { set; get; }
            public User(string name = "John", int age = 25) {
                Name = name;
                Age = age;
            }
        }

        public ContextTest(BemolServerFixture server) {
            Server = server;
        }

        // ********************************************************************************************
        // REQUEST
        // ********************************************************************************************

        [Fact]
        public void Body_String_Equal() {
            var content = new StringContent("Hello world!");
            var ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Equal("Hello world!", ctx.Body());
        }

        [Fact]
        public void Body_Json_Equal() {
            var content = JsonContent.Create(new { Name = "John", Age = 25 });
            var ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Equal("{\"name\":\"John\",\"age\":25}", ctx.Body());
        }

        [Fact]
        public void Body_FormData_Equal() {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(KeyValuePair.Create("foo", "bar"));
            form.Add(KeyValuePair.Create("baz", "foobar"));
            var formData = new FormUrlEncodedContent(form);
            var ctx = Server.GetContext(client => client.PostAsync("/", formData));
            Assert.Equal("foo=bar&baz=foobar", ctx.Body());
        }

        [Fact]
        public void BodyAsBytes_Equal() {
            var content = new StringContent("Hello world!");
            var ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Equal(Encoding.UTF8.GetBytes("Hello world!"), ctx.BodyAsBytes());
        }

        [Fact]
        public void BodyAsClass_Equal() {
            var user = new User();
            HttpContent content = JsonContent.Create(user); // type has to be HttpContent, else it'll hang indefinitely
            var ctx = Server.GetContext(client => client.PostAsJsonAsync("/", content));
            var returned = ctx.BodyAsClass<User>();
            Assert.Equal(user.Age, returned.Age);
            Assert.Equal(user.Name, returned.Name);
        }

        [Fact]
        public void BodyAsClass_NotValid() {
            var ctx = Server.GetContext(client => client.PostAsync("/", null));
            Assert.Throws<BadRequestException>(() => ctx.BodyAsClass<User>());
        }

        [Fact]
        public void FormParam_Equal() {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(KeyValuePair.Create("foo", "bar"));
            form.Add(KeyValuePair.Create("baz", "foobarzé"));
            var formData = new FormUrlEncodedContent(form);
            var ctx = Server.GetContext(client => client.PostAsync("/", formData));
            Assert.Equal("bar", ctx.FormParam("foo"));
            Assert.Equal("foobarzé", ctx.FormParam("baz"));
        }

        [Fact]
        public void FormParam_FormIsNull_IsNull() {
            var ctx = Server.GetContext(client => client.PostAsync("/", null));
            Assert.Null(ctx.FormParam("foo"));
        }

        [Fact]
        public void FormParam_MultipleValuesForSameKey_Equal() {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(KeyValuePair.Create("foo", "bar"));
            form.Add(KeyValuePair.Create("foo", "baz"));
            var formData = new FormUrlEncodedContent(form);
            var ctx = Server.GetContext(client => client.PostAsync("/", formData));
            Assert.Equal("bar,baz", ctx.FormParam("foo"));
        }

        [Fact]
        public void ContentType_TextPlain_Equal() {
            var content = new StringContent("Hello world!");
            var ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Matches("text/plain", ctx.ContentType());
        }

        [Fact]
        public void ContentType_ApplicationJson_Equal() {
            var content = JsonContent.Create(new User());
            var ctx = Server.GetContext(client => client.PostAsync("/", content));
            Assert.Matches("application/json", ctx.ContentType());
        }

        [Theory]
        [InlineData("foo", "bar")]
        public void Cookie_GetCookie_Equal(string name, string value) {
            var ctx = Server.GetContext(client => {
                client.DefaultRequestHeaders.Add("Cookie", $"{name}={value}");
                client.GetAsync("/");
            });
            Assert.Equal(value, ctx.Cookie(name));
        }

        [Fact]
        public void Cookie_GetCookie_IsNull() {
            var ctx = Server.GetContext(client => client.GetAsync("/"));
            Assert.Null(ctx.Cookie("bar"));
        }

        [Theory]
        [InlineData("foo", "bar")]
        public void Header_Equal(string name, string value) {
            var ctx = Server.GetContext(client => {
                client.DefaultRequestHeaders.Add(name, value);
                client.GetAsync("/");
            });
            Assert.Equal(value, ctx.Header(name));
        }

        [Fact]
        public void Header_NameDoesntExist_IsNull() {
            var ctx = Server.GetContext(client => client.GetAsync("/"));
            Assert.Null(ctx.Header("foo"));
        }

        [Fact]
        public void Method_Get_Equal() {
            var ctx = Server.GetContext(client => client.GetAsync("/"));
            Assert.Equal("GET", ctx.Method());
        }

        [Fact]
        public void Method_Post_Equal() {
            var ctx = Server.GetContext(client => client.PostAsync("/", null));
            Assert.Equal("POST", ctx.Method());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/user")]
        [InlineData("/user/")]
        public void Path_Equal(string path) {
            var ctx = Server.GetContext(client => client.GetAsync(path));
            Assert.Equal(path, ctx.Path());
        }

        [Theory]
        [InlineData("/?foo=bar", "foo", "bar")]
        [InlineData("/?foo=bar&foo=baz", "foo", "bar,baz")]
        public void QueryMap_Equal(string url, string key, string value) {
            var ctx = Server.GetContext(client => client.GetAsync(url));
            Assert.Equal(value, ctx.QueryMap()[key]);
        }

        [Theory]
        [InlineData("/hello?foo=bar", "?foo=bar")]
        public void QueryString_Equal(string url, string expected) {
            var ctx = Server.GetContext(client => client.GetAsync(url));
            Assert.Equal(expected, ctx.QueryString());
        }

        [Fact]
        public void UserAgent_Normal_Equal() {
            var ctx = Server.GetContext(client => {
                client.DefaultRequestHeaders.Add("User-Agent", "Foo");
                client.GetAsync("/");
            });
            Assert.Equal("Foo", ctx.UserAgent());
        }

        [Fact]
        public void UserAgent_IsNull_Equal() {
            var ctx = Server.GetContext(client => client.GetAsync("/"));
            Assert.Null(ctx.UserAgent());
        }

        // ********************************************************************************************
        // RESPONSE
        // ********************************************************************************************

        [Theory]
        [InlineData("Hello world!")]
        [InlineData("Tommy Josépovic")]
        [InlineData("На берегу пустынных волн")]
        [InlineData("ვეპხის ტყაოსანი შოთა რუსთაველი")]
        [InlineData("⠊⠀⠉⠁⠝⠀⠑⠁⠞⠀⠛⠇⠁⠎⠎⠀⠁⠝⠙⠀⠊⠞⠀⠙⠕⠑⠎⠝⠞⠀⠓⠥⠗⠞⠀⠍⠑")]
        public void ResultString_SetUsingString_Equal(string result) {
            var ctx = Server.GetContext(client => client.GetAsync("/"));
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
            var ctx = Server.GetContext(client => client.GetAsync("/"));
            byte[] byteArray = Encoding.UTF8.GetBytes(result);
            ctx.Result(byteArray);
            Assert.Equal(result, ctx.ResultString());
        }

        [Theory]
        [InlineData(404)]
        [InlineData(500)]
        public void Status_SetUsingStatus_Equal(int status) {
            var ctx = Server.GetContext(async client => {
                var response = await client.GetAsync("/");
                Assert.Equal(status, (int)response.StatusCode);
            });
            ctx.Status(status);
            Server.SendResponse(ctx);
        }

        [Theory]
        [InlineData("baz", "for")]
        public void Cookie_SetNameValue_Equal(string name, string value) {
            var ctx = Server.GetContext(async client => {
                var response = await client.GetAsync("/");
                var cookies = response.Headers.GetValues("Set-Cookie");
                foreach (var cookie in cookies) {
                    Assert.Equal($"{name}={value}", cookie);
                }
            });
            ctx.Cookie(name, value);
            Server.SendResponse(ctx);
        }

        [Theory]
        [InlineData("for", "baz")]
        public void Cookie_SetCookie_Equal(string name, string value) {
            var ctx = Server.GetContext(async client => {
                var response = await client.GetAsync("/");
                var cookies = response.Headers.GetValues("Set-Cookie");
                foreach (var cookie in cookies) {
                    Assert.Equal($"{name}={value}", cookie);
                }
            });
            ctx.Cookie(new Cookie(name, value));
            Server.SendResponse(ctx);
        }

        [Theory]
        [InlineData("baz", "buzz")]
        public void RemoveCookie_Expired(string name, string value) {
            var ctx = Server.GetContext(async client => {
                client.DefaultRequestHeaders.Add("Cookie", $"{name}={value}");
                var response = await client.GetAsync("/");
                var cookies = response.Headers.GetValues("Set-Cookie");
                foreach (var cookie in cookies) {
                    Assert.Equal($"{name}=; Max-Age=0", cookie);
                }
            });
            ctx.RemoveCookie(name);
            Server.SendResponse(ctx);
        }

        [Fact]
        public void Json_SimpleObject_Equal() {
            var ctx = Server.GetContext(client => client.GetAsync("/"));
            ctx.Json(new { Name = "John", Age = 10 });
            Assert.Equal("{\"Name\":\"John\",\"Age\":10}", ctx.ResultString());
        }
    }
}