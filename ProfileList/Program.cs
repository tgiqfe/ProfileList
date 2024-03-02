using ProfileList;
using ProfileList.Lib;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Item.MachineInfo = new MachineInfo();
UserProfileCollection collection = new();

app.MapGet("/", () => "");
app.MapPost("/", () => "");

app.MapGet("/api/list", () =>
{
    return collection;
});

app.MapGet("/api/machine", () =>
{
    return new
    {
        ComputerName = Item.MachineInfo.ComputerName,
        DomainName = Item.MachineInfo.DomainName,
        IsDomainMachine = Item.MachineInfo.IsDomainMachine,
        SystemSIDs = Item.MachineInfo.SystemSIDs
    };
});

app.MapGet("/api/session", () =>
{
    return new
    {
        Sessions = Item.MachineInfo.UserLogonSessions,
    };
});

app.MapGet("/api/reflesh", () =>
{
    Item.MachineInfo = new MachineInfo();
    collection = new UserProfileCollection();
    return new
    {
        Result = "OK"
    };
});



app.Run("http://*:5000");
