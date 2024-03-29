using ProfileList2;

Item.Setting = Setting.Load();
Item.Setting.RegisterEnvironment();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "");

RoutingManager routing = new(app);
routing.RegisterRoutes_app();
routing.RegisterRoutes_v1();


app.Run($"http://*:{Item.Setting.Port}");
