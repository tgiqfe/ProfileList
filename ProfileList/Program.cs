using Microsoft.Extensions.Primitives;
using ProfileList;
using ProfileList.Lib.Profile;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using ProfileList.Lib;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region Static parameter set

Item.Setting = Setting.Load();
Item.MachineInfo = new();
Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
Item.UserProfileCollection = new();
Item.Logger = new(Item.Setting.LogDirectory);

#endregion

app.MapGet("/", () => "");
app.MapPost("/", () => "");

RoutingManager routing = new(app);
routing.RegisterRoutes();

app.MapPost("/api/user/test", async (HttpContext context) =>
{
    switch (context.Request.ContentType)
    {
        case "application/json":
            Item.Logger.WriteLine("application/json");
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var node = JsonNode.Parse(body);
                var username = node["username"]?.ToString();
                var password = node["password"]?.ToString();
                var domainname = node["domain"]?.ToString();
                Item.Logger.WriteLine($"{username} {password} {domainname}");
            }
            break;
        case "application/x-www-form-urlencoded":
            Item.Logger.WriteLine("application/x-www-form-urlencoded");
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                string username = "", password = "", domainname = "";
                foreach (var parameter in body.Split("&"))
                {
                    var key = parameter.Substring(0, parameter.IndexOf("="));
                    var val = parameter.Substring(parameter.IndexOf("=") + 1);
                    switch (key)
                    {
                        case "username":
                            username = val;
                            break;
                        case "password":
                            password = val;
                            break;
                        case "domain":
                            domainname = val;
                            break;
                    }
                }
                Item.Logger.WriteLine($"{username} {password} {domainname}");
            }
            break;
    }

    return new
    {
        Result = "OK"
    };
});

app.Run($"http://*:{Item.Setting.Port}");
