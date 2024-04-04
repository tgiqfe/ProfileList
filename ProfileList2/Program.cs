using ProfileList2;
using ProfileList2.Lib.ScriptLanguage;
using System.Diagnostics;

Item.WorkingDirectory = Path.GetDirectoryName(
    Process.GetCurrentProcess().MainModule.FileName);
Item.LanguageCollection = LanguageCollection.Load(
    Path.Combine(Item.WorkingDirectory, "languages.json"));
Item.Setting = Setting.Load();
Item.Setting.RegisterEnvironment();

if (!string.IsNullOrEmpty(Item.Setting.PwshPath))
{
    Item.LanguageCollection.Languages.First(x => x.Name == "Pwsh7").Command =
        Item.Setting.PwshPath;
}
if (!string.IsNullOrEmpty(Item.Setting.PythonPath))
{
    Item.LanguageCollection.Languages.First(x => x.Name == "Python").Command =
        Item.Setting.PythonPath;
}




var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "");

RoutingManager routing = new(app);
routing.RegisterRoutes_app();
routing.RegisterRoutes_v1();


app.Run($"http://*:{Item.Setting.Port}");
