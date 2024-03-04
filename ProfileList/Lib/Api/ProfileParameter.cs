using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class ProfileParameter : ApiParameter
    {
        /// <summary>
        /// プロファイルに関する処理の前に、プロファイル情報を更新する。
        /// </summary>
        public bool? Refresh { get; set; }

        /// <summary>
        /// 対象のユーザー名。プロファイル削除時に指定する。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// プロファイル削除時に、全ユーザーのプロファイルを削除する。
        /// </summary>
        public bool? All { get; set; }

        public ProfileParameter() { }

        /*
        public static async Task<ProfileParameter> SetParamAsync(HttpContext context)
        {
            ProfileParameter parameter = null;
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
                    parameter.Refresh = bool.TryParse(node["refresh"]?.ToString(), out bool refresh_1) ? refresh_1 : null;
                    parameter.UserName = node["username"]?.ToString();
                    parameter.All = bool.TryParse(node["all"]?.ToString(), out bool all_1) ? all_1 : null;
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
                            case "refresh":
                                parameter.Refresh = bool.TryParse(val, out bool refresh_2) ? refresh_2 : null;
                                break;
                            case "username":
                                parameter.UserName = val;
                                break;
                            case "all":
                                parameter.All = bool.TryParse(val, out bool all_2) ? all_2 : null;
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
