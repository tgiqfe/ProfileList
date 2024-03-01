using ProfileList;
using ProfileList.Lib;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

UserProfileCollection collection = new();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/list", () =>
{
    return collection;
});

app.MapGet("/api/machine", () =>
{
    return new
    {
        ComputerName = MachineInfo.ComputerName,
        DomainName = MachineInfo.DomainName,
        IsDomainMachine = MachineInfo.IsDomainMachine,
        SystemSIDs = MachineInfo.SystemSIDs
    };
}); 




app.Run();
