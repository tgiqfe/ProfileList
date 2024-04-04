using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProfileList2.App
{
    public class ApplicationProcess
    {
        public static dynamic Close(WebApplication app)
        {
            Task.Run(() =>
            {
                Task.Delay(1000);
                app.StopAsync();
            });
            return new
            {
                Status = "Close",
                Result = JsonNode.Parse("{}")
            };
        }

        public static dynamic GetProcessInfo()
        {
            var info = new
            {
                MachineName = Environment.MachineName,
                User = Environment.UserName,
                ProcessPath = Process.GetCurrentProcess().MainModule.FileName
            };
            return new
            {
                Status = "OK",
                Result = JsonNode.Parse(JsonSerializer.Serialize(info))
            };
        }
    }
}
