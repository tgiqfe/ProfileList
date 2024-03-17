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

                return ExecuteScript(targetDir, metadataPath, argsText, "GET");
            });

            //  API v1 (POST)
            app.MapPost("/api/v1/{scriptGroup}/{scriptName}", async (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                var argsText = await ToArgsTextAsync(context);

                return await ExecuteScriptAsync(targetDir, metadataPath, argsText, "POST");
            });

            //  API v1 (PUT)
            app.MapPut("/api/v1/{scriptGroup}/{scriptName}", async (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                var argsText = await ToArgsTextAsync(context);

                return await ExecuteScriptAsync(targetDir, metadataPath, argsText, "PUT");
            });

            //  API v1 (DELETE)
            app.MapDelete("/api/v1/{scriptGroup}/{scriptName}", async (string scriptGroup, string scriptName, HttpContext context) =>
            {
                var targetDir = Path.Combine(Item.WorkingDirectory, "v1", scriptGroup, scriptName);
                var metadataPath = Path.Combine(targetDir, "metadata.yml");
                var argsText = await ToArgsTextAsync(context);

                return await ExecuteScriptAsync(targetDir, metadataPath, argsText, "DELETE");
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
        /// <param name="method"></param>
        /// <returns></returns>
        private dynamic ExecuteScript(string targetDir, string metadataPath, string argsText, string method)
        {
            return ExecuteScriptAsync(targetDir, metadataPath, argsText, method).Result;
        }

        /// <summary>
        /// MetadataPathからスクリプトを実行。Async版
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="metadataPath"></param>
        /// <param name="argsText"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private async Task<dynamic> ExecuteScriptAsync(string targetDir, string metadataPath, string argsText, string method)
        {
            //  メタデータの有無を確認
            if (!File.Exists(metadataPath))
            {
                return new
                {
                    Status = "Empty",
                    Result = JsonNode.Parse("{}"),
                };
            }

            //  HTTPメソッドが一致するか確認
            var metadata = MetadataFile.Load(metadataPath)?.Metadata;
            if (!metadata.IsMatchMethod(method))
            {
                return new
                {
                    Status = "MethodMismatch",
                    Result = JsonNode.Parse(JsonSerializer.Serialize(new
                    {
                        RequestMethod = method,
                        ConfigedMethod = metadata.Method,
                    })),
                };
            }

            //  スクリプトファイルの有無を確認
            var scriptPath = Path.Combine(targetDir, metadata.ScriptPath);
            if (!File.Exists(scriptPath))
            {
                return new
                {
                    Status = "ScriptMissing",
                    Result = JsonNode.Parse("{}"),
                };
            }

            //  スクリプトを実行
            try
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
