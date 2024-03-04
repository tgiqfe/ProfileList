using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class UserParameter : ApiParameter
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }
        public bool? Refresh { get; set; }

        public UserParameter() { }

        /*
        public static async Task<UserParameter> SetParamAsync(HttpContext context)
        {
            UserParameter parameter = null;
            string body = "";
            using (var reader = new StreamReader(context.Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            switch (context.Request.ContentType)
            {
                case "application/json":
                    Item.Logger.WriteLine("Content-Type: application/json");
                    var node = JsonNode.Parse(body);
                    parameter = new();
                    parameter.UserName = node["username"]?.ToString();
                    parameter.Password = node["password"]?.ToString();
                    parameter.DomainName = node["domainame"]?.ToString();
                    parameter.Refresh = bool.TryParse(node["refresh"]?.ToString(), out bool refresh_1) ? refresh_1 : null;
                    break;
                case "application/x-www-form-urlencoded":
                    Item.Logger.WriteLine("Content-Type: application/x-www-form-urlencoded");
                    parameter = new();
                    foreach (var leaf in body.Split("&"))
                    {
                        var key = leaf.Substring(0, leaf.IndexOf("="));
                        var val = leaf.Substring(leaf.IndexOf("=") + 1);
                        switch (key)
                        {
                            case "username":
                                parameter.UserName = val;
                                break;
                            case "password":
                                parameter.Password = val;
                                break;
                            case "domain":
                            case "domainame":
                                parameter.DomainName = val;
                                break;
                            case "refresh":
                                parameter.Refresh = bool.TryParse(val, out bool refresh_2) ? refresh_2 : null;
                                break;
                        }
                    }
                    break;
                default:
                    Item.Logger.WriteLine($"Content-Type is no supported type. [{context.Request.ContentType}]");
                    break;
            }

            return parameter;
        }
        */
    }
}
