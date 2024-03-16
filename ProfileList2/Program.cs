using ProfileList2;
using ProfileList2.Lib;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/v1/{scriptGroup}/{scriptName}", (string scriptGroup, string scriptName) =>
{
    var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
    var metadataPath = Path.Combine(targetDir, "metadata.yml");
    if (File.Exists(metadataPath))
    {
        var metadata = MetadataFile.Load(metadataPath)?.Metadata;
        var scriptPath = Path.Combine(targetDir, metadata.ScriptPath);
        if (File.Exists(scriptPath))
        {
            StringBuilder sb = new();
            using (var proc = Item.LanguageCollection.GetProcess(scriptPath))
            {
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WorkingDirectory = targetDir;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.OutputDataReceived += (sender, e) => sb.AppendLine(e.Data);
                proc.ErrorDataReceived += (sender, e) => sb.AppendLine(e.Data);
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
            }
            Debug.WriteLine(sb.ToString());
            var node = JsonNode.Parse(sb.ToString());

            return new
            {
                Result = "OK",
                Script = node,
            };
        }
    }

    return new
    {
        Result = "OK",
        Script = JsonNode.Parse("{}"),
    };
});




app.Run("http://*:5000");
