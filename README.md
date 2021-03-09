# Bemol: a simple web framework for C#

Bemol's main goal is simplicity. You don't have to extend anything, there are no @Annotations, there is no magic; just code.

- Download: https://www.nuget.org/packages/Bemol/
- Documentation: https://bemol-docs.web.app/

## Quickstart

### Add dependency

#### .NET CLI

```
dotnet add package Bemol --version 1.1.0-alpha
```

#### .csproj

```xml
<ItemGroup>
    <PackageReference Include="Bemol" Version="1.1.0-alpha" />
</ItemGroup>
```

### Start coding:

```cs
using Bemol;

class HelloWorld {
    static void Main() {
        App app = new App().Start(7000);
        app.Get("/", (ctx) => ctx.Result("Hello World!"));
    }
}
```

### Run the development server:

```
dotnet watch run
```

## Examples

### JSON API

```cs
App app = new App(config => {
    config.DefaultContentType = "application/json";
    config.EnableCorsForAllOrigin = true;
}).Start();

var todos = List<Todo>();
app.Get("/todos", ctx => { // map array of Todos to json-string
    ctx.Json(todos);
});
app.Put("/todos", ctx => { // map request-body (json) to array of Todos
    todos = ctx.Body<List<Todo>>();
    ctx.Status(204);
});

```

### File upload

```cs
App app = new App().Start();

app.Post("/upload", ctx => {
    var file = ctx.UploadedFile("file");
    using (var fs = File.Create($"./upload/{file.Filename}")) {
        file.Content.Seek(0, SeekOrigin.Begin);
        file.Content.CopyTo(fs);
    }
}
```

## Special thanks

- David Åse, for creating [Javalin](https://javalin.io/).
- Blake Mizerany, for creating [Sinatra](http://www.sinatrarb.com/).
- Per Wendel, for creating [Spark](http://sparkjava.com/).
