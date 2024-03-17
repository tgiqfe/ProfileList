using ProfileList2.Lib;
using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;

namespace ProfileList2
{
    public class RoutingManager
    {
        private WebApplication app;

        public RoutingManager(WebApplication _app)
        {
            app = _app;
        }

        public void RegisterRoutes()
        {
            //  API v1 (GET)
            app.MapGet("/api/v1/{scriptGroup}/{scriptName}", (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                string argsText = ToArgsText(context);

                return ExecuteScript(targetDir, metadataPath, argsText);
            });

            //  API v1 (POST)
            app.MapPost("/api/v1/{scriptGroup}/{scriptName}", async (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                var argsText = await ToArgsTextAsync(context);

                return await ExecuteScriptAsync(targetDir, metadataPath, argsText);
            });

            //  API v1 (PUT)
            app.MapPut("/api/v1/{scriptGroup}/{scriptName}", async (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                var argsText = await ToArgsTextAsync(context);

                return await ExecuteScriptAsync(targetDir, metadataPath, argsText);
            });

            //  API v1 (DELETE)
            app.MapDelete("/api/v1/{scriptGroup}/{scriptName}", async (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                var argsText = await ToArgsTextAsync(context);

                return await ExecuteScriptAsync(targetDir, metadataPath, argsText);
            });
        }

        /// <summary>
        /// HttpContextのクエリパラメータから、スクリプトに渡す引数を生成する
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ToArgsText(HttpContext context)
        {
            return string.Join(" ",
                context.Request.Query.ToList().Select(x => $"-{x.Key} {x.Value}"));
        }

        /// <summary>
        /// HttpContextのBodyから、スクリプトに渡す引数を生成する
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<string> ToArgsTextAsync(HttpContext context)
        {
            string body = "";
            using (var reader = new StreamReader(context.Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            if (!string.IsNullOrEmpty(body))
            {
                string contentType = context.Request.ContentType.
                Split(";").
                Select(x => x.Trim()).
                FirstOrDefault(x => x.StartsWith("application/"));

                switch (contentType)
                {
                    case "application/json":
                        var node = JsonNode.Parse(
                            body,
                            new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
                        return string.Join(" ", node.AsObject().Select(x => $"-{x.Key} {x.Value}"));
                    case "application/x-www-form-urlencoded":
                        return string.Join(" ", body.Split("&").Select(x =>
                        {
                            var pair = x.Split("=");
                            return $"-{pair[0]} {pair[1]}";
                        }));
                }
            }
            return "";
        }

        /// <summary>
        /// MetadataPathからスクリプトを実行する
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="metadataPath"></param>
        /// <param name="argsText"></param>
        /// <returns></returns>
        private dynamic ExecuteScript(string targetDir, string metadataPath, string argsText)
        {
            try
            {
                if (File.Exists(metadataPath))
                {
                    var metadata = MetadataFile.Load(metadataPath)?.Metadata;
                    var scriptPath = Path.Combine(targetDir, metadata.ScriptPath);
                    if (File.Exists(scriptPath))
                    {
                        StringBuilder output = new();
                        using (var proc = Item.LanguageCollection.GetProcess(scriptPath, argsText))
                        {
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.WorkingDirectory = targetDir;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.OutputDataReceived += (sender, e) => output.AppendLine(e.Data);
                            proc.Start();
                            proc.BeginOutputReadLine();
                            proc.WaitForExit();
                        }
                        return new
                        {
                            Status = "OK",
                            Result = JsonNode.Parse(output.ToString()),
                        };
                    }
                }
                return new
                {
                    Status = "Empty",
                    Result = JsonNode.Parse("{}"),
                };
            }
            catch (Exception e)
            {
                return new
                {
                    Status = "NG",
                    Result = JsonNode.Parse(JsonSerializer.Serialize(new
                    {
                        Message = e.Message,
                        StackTrace = e.StackTrace,
                    })),
                };
            }
        }

        /// <summary>
        /// MetadataPathからスクリプトを実行。Async版
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="metadataPath"></param>
        /// <param name="argsText"></param>
        /// <returns></returns>
        private async Task<dynamic> ExecuteScriptAsync(string targetDir, string metadataPath, string argsText)
        {
            try
            {
                if (File.Exists(metadataPath))
                {
                    var metadata = MetadataFile.Load(metadataPath)?.Metadata;
                    var scriptPath = Path.Combine(targetDir, metadata.ScriptPath);
                    if (File.Exists(scriptPath))
                    {
                        StringBuilder sb = new();
                        using (var proc = Item.LanguageCollection.GetProcess(scriptPath, argsText))
                        {
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.WorkingDirectory = targetDir;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.OutputDataReceived += (sender, e) => sb.AppendLine(e.Data);
                            proc.Start();
                            proc.BeginOutputReadLine();
                            await proc.WaitForExitAsync();
                        }
                        return new
                        {
                            Status = "OK",
                            Result = JsonNode.Parse(sb.ToString()),
                        };
                    }
                }
                return new
                {
                    Status = "Empty",
                    Result = JsonNode.Parse("{}"),
                };
            }
            catch (Exception e)
            {
                return new
                {
                    Status = "NG",
                    Result = JsonNode.Parse(JsonSerializer.Serialize(new
                    {
                        Message = e.Message,
                        StackTrace = e.StackTrace,
                    })),
                };
            }
        }

    }
}
