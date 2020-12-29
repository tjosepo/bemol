using Bemol;
using System.Collections.Generic;
using Bemol.DotLiquid;

public class Program {
    public static void Main() {
        App app = new App(config => {
            config.DefaultContentType = "application/json";
        }).Start();

        var list = new UsersList();
        list.Users.Add(new User("Johnny", 20));
        list.Users.Add(new User("Bob", 23));


        app.Get("/", ctx => {
            ctx.Render("/index.liquid", list);
        });
    }
}


class UsersList {
    public List<User> Users { set; get; } = new List<User>();
}

class User : Drop {
    public string Name { get; }
    public int Age { set; get; }
    public int BirthPlace { set; get; } = 99;

    public User(string name, int age) {
        Name = name;
        Age = age;
    }
}