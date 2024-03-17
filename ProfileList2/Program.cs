using ProfileList2;
using ProfileList2.Lib;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "");

RoutingManager routing = new(app);
routing.RegisterRoutes();

app.Run("http://*:5000");
