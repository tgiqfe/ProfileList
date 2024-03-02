using Microsoft.AspNetCore.Mvc.ApplicationParts;
using ProfileList;
using ProfileList.Lib;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Item.Setting = Setting.Load();
Item.MachineInfo = new();
Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
Item.UserProfileCollection = new();

app.MapGet("/", () => "");
app.MapPost("/", () => "");

app.MapGet("/api/profile/list", () =>
{
    return Item.UserProfileCollection;
});

app.MapGet("/api/session/list", () =>
{
    Item.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
    return new
    {
        Sessions = Item.UserLogonSessions,
    };
});

app.MapPost("/api/user/logon", (HttpContext context) =>
{
    var ret_logon = ConsoleLogon.CheckLogonUser("Administrator", "", "");
    if (ret_logon)
    {
        var ret_agent = ConsoleLogon.CheckRunningAgent();
        if (ret_agent)
        {
            ConsoleLogon.Enter("Administrator", "", "");
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
    Item.MachineInfo = new();
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
    Item.MachineInfo = new();
    Item.UserLogonSessions.
        Where(x => x.ProtocolType == 2).
        ToList().
        ForEach(x => x.Disconnect());
    return new
    {
        Result = "OK"
    };
});

app.MapGet("/api/system/refresh", () =>
{
    Item.MachineInfo = new();
    Item.UserProfileCollection = new();
    return new
    {
        Result = "OK"
    };
});

app.MapGet("/api/system/info", () =>
{
    return new
    {
        ComputerName = Item.MachineInfo.ComputerName,
        DomainName = Item.MachineInfo.DomainName,
        IsDomainMachine = Item.MachineInfo.IsDomainMachine,
        SystemSIDs = Item.MachineInfo.SystemSIDs
    };
});

app.MapGet("/api/system/close", () =>
{
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


app.Run("http://*:5000");
