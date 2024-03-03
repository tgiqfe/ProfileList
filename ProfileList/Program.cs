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
Item.NetworkProfile = new();
Item.UserLogonSessionCollection = new();
Item.UserProfileCollection = new();
Item.Logger = new(Item.Setting.LogDirectory);

#endregion

Item.Logger.WriteLine("Start Service.");

app.MapGet("/", () => "");
app.MapPost("/", () => "");

RoutingManager routing = new(app);
routing.RegisterRoutes();

app.Run($"http://*:{Item.Setting.Port}");
