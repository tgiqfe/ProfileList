using System.Reflection;
using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class ApiParameter
    {
        public ApiParameter() { }

        public static async Task<T> SetAsync<T>(HttpContext context) where T : ApiParameter, new()
        {
            T parameter = null;
            string body = "";
            using (var reader = new StreamReader(context.Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            if (string.IsNullOrEmpty(body))
            {
                Item.Logger.WriteLine("Request body is empty.");
                return parameter;
            }

            var props = typeof(T).GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            switch (context.Request.ContentType)
            {
                case "application/json":
                    Item.Logger.WriteLine("Content-Type: application/json");
                    var node = JsonNode.Parse(
                        body,
                        new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
                    parameter = new();
                    foreach (var prop in props)
                    {
                        var val = node[prop.Name]?.ToString();
                        if (val != null)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(parameter, val);
                            }
                            else if (prop.PropertyType == typeof(int?))
                            {
                                prop.SetValue(parameter, int.TryParse(val, out int i) ? i : null);
                            }
                            else if (prop.PropertyType == typeof(bool?))
                            {
                                prop.SetValue(parameter, bool.TryParse(val, out bool b) ? b : null);
                            }
                        }
                    }
                    break;
                case "application/x-www-form-urlencoded":
                    Item.Logger.WriteLine("Content-Type: application/x-www-form-urlencoded");
                    var leaves = body.Split("&");
                    parameter = new();
                    foreach (var leaf in leaves)
                    {
                        var key = leaf.Substring(0, leaf.IndexOf("="));
                        var val = leaf.Substring(leaf.IndexOf("=") + 1);
                        var prop = props.FirstOrDefault(x => x.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(parameter, val);
                            }
                            else if (prop.PropertyType == typeof(int?))
                            {
                                prop.SetValue(parameter, int.TryParse(val, out int i) ? i : null);
                            }
                            else if (prop.PropertyType == typeof(bool?))
                            {
                                prop.SetValue(parameter, bool.TryParse(val, out bool b) ? b : null);
                            }
                        }
                    }
                    break;
                default:
                    Item.Logger.WriteLine($"Content-Type is no supported type. [{context.Request.ContentType}]");
                    break;
            }

            return parameter;
        }
    }
}
