using ProfileList;
using ProfileList.Lib;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region Static parameter set

Item.Setting = Setting.Load();
Item.MachineInfo = new();
Item.NetworkProfile = new();
Item.UserLogonSessionCollection = new();
Item.UserProfileCollection = new();
Item.Logger = new();

#endregion
#region Start log

Item.Logger.WriteLine("Start Service.");
Item.Logger.WriteLine($"Setting: Port [{Item.Setting.Port}]");
Item.Logger.WriteLine($"Setting: RLAgentPipeKey [{Item.Setting.RLAgentPipeKey}]");
Item.Logger.WriteLine($"Setting: RLAgentMutexKey [{Item.Setting.RLAgentMutexKey}]");
Item.Logger.WriteLine($"Setting: LogDirectory [{Item.Setting.LogDirectory}]");
Item.Logger.WriteLine($"Setting: ProtectedProfile [{Item.Setting.ProtectedProfile}]");

#endregion

app.MapGet("/", () => "");
app.MapPost("/", () => "");

RoutingManager routing = new(app);
routing.RegisterRoutes();

app.Run($"http://*:{Item.Setting.Port}");
