using ProfileList;
using ProfileList.Lib;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Item.MachineInfo = new();
Item.UserProfileCollection = new();

app.MapGet("/", () => "");
app.MapPost("/", () => "");

app.MapGet("/api/profile/list", () =>
{
    return Item.UserProfileCollection;
});

app.MapGet("/api/info/machine", () =>
{
    return new
    {
        ComputerName = Item.MachineInfo.ComputerName,
        DomainName = Item.MachineInfo.DomainName,
        IsDomainMachine = Item.MachineInfo.IsDomainMachine,
        SystemSIDs = Item.MachineInfo.SystemSIDs
    };
});

app.MapGet("/api/info/session", () =>
{
    return new
    {
        Sessions = Item.MachineInfo.UserLogonSessions,
    };
});

app.MapGet("/api/info/reflesh", () =>
{
    Item.MachineInfo = new();
    Item.UserProfileCollection = new();
    return new
    {
        Result = "OK"
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
