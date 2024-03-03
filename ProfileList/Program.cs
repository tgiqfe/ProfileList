using Microsoft.Extensions.Primitives;
using ProfileList;
using ProfileList.Lib;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Item.Setting = Setting.Load();
Item.MachineInfo = new();
Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
Item.UserProfileCollection = new();
Item.Logger = new(Item.Setting.LogDirectory);

app.MapGet("/", () => "");
app.MapPost("/", () => "");

app.MapGet("/api/profile/list", () =>
{
    Item.Logger.WriteLine("Get profile list.");
    return Item.UserProfileCollection;
});

app.MapGet("/api/session/list", () =>
{
    Item.Logger.WriteLine("Get Session list.");
    Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
    return new
    {
        Sessions = Item.UserLogonSessions,
    };
});





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





app.MapPost("/api/user/logon", async (HttpContext context) =>
{
    Item.Logger.WriteLine("User Logon.");

    string username = "", password = "", domainname = "";
    switch (context.Request.ContentType)
    {
        case "application/json":
            Item.Logger.WriteLine("application/json");
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var node = JsonNode.Parse(body);
                username = node["username"]?.ToString();
                password = node["password"]?.ToString();
                domainname = node["domain"]?.ToString();
                Item.Logger.WriteLine($"{username} {password} {domainname}");
            }
            break;
        case "application/x-www-form-urlencoded":
            Item.Logger.WriteLine("application/x-www-form-urlencoded");
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();

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
        default:
            return new
            {
                Result = "NG",
                Description = "Not support Content-Type."
            };
    }

    var ret_logon = ConsoleLogon.CheckLogonUser(username, password, domainname);
    if (ret_logon)
    {
        var ret_agent = ConsoleLogon.CheckRunningAgent();
        if (ret_agent)
        {
            ConsoleLogon.Enter(username, password, domainname);
            return new
            {
                Result = "OK",
                Description = "Logon success."
            };
        }
        else
        {
            return new
            {
                Result = "NG",
                Description = "Agent stop."
            };
        }
    }
    else
    {
        return new
        {
            Result = "NG",
            Description = "Failed to logon."
        };
    }
});

app.MapGet("/api/user/logoff", () =>
{
    Item.Logger.WriteLine("User Logoff.");
    Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
    Item.UserLogonSessions.
        ToList().
        ForEach(x => x.Logoff());
    return new
    {
        Result = "OK"
    };
});

app.MapGet("api/user/disconnect", () =>
{
    Item.Logger.WriteLine("User Disconnect, from RDP.");
    Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
    Item.UserLogonSessions.
        Where(x => x.ProtocolType == 2).
        ToList().
        ForEach(x => x.Disconnect());
    return new
    {
        Result = "OK"
    };
});

app.MapGet("/api/log/print", () =>
{
    Item.Logger.WriteLine("Log print.");
    return new
    {
        Log = Item.Logger.Print()
    };
});

app.MapGet("/api/system/refresh", () =>
{
    Item.Logger.WriteLine("System Refresh.");
    Item.MachineInfo = new();
    Item.UserProfileCollection = new();
    return new
    {
        Result = "OK"
    };
});

app.MapGet("/api/system/info", () =>
{
    Item.Logger.WriteLine("Get System Info.");
    return new
    {
        MachineInfo = Item.MachineInfo,
    };
});

app.MapGet("/api/system/close", () =>
{
    Item.Logger.WriteLine("Close Application.");
    Task.Run(() =>
    {
        Task.Delay(1000);
        app.StopAsync();
    });
    return new
    {
        Result = "OK"
    };
});


app.Run($"http://*:{Item.Setting.Port}");
