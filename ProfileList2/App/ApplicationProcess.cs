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
    }
}
