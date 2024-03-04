using ProfileList.Lib.Api;
using System.Text.Json.Nodes;

namespace ProfileList.Lib
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
            //  プロファイル一覧を取得
            app.MapGet("/api/user/profile", () =>
            {
                Item.Logger.WriteLine("[GET]Get user profile list.");
                return Api.User.Profile();
            });
            app.MapPost("/api/user/profile", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Get user profile list.");
                return Api.User.Profile(await UserParameter.SetParamAsync(context));
            });

            //  ログイン中セッションの一覧を取得
            app.MapGet("/api/user/session", () =>
            {
                Item.Logger.WriteLine("[GET]Get user logon sessions.");
                return Api.User.Session();
            });
            app.MapPost("/api/user/session", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Get user logon sessions.");
                return Api.User.Session(await UserParameter.SetParamAsync(context));
            });






            //  ユーザーのログオン
            app.MapPost("/api/user/logon", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]User Logon.");

                string username = "", password = "", domainname = "";
                switch (context.Request.ContentType)
                {
                    case "application/json":
                        Item.Logger.WriteLine("application/json");
                        using (var reader = new StreamReader(context.Request.Body))
                        {
                            var body = await reader.ReadToEndAsync();
                            var node = JsonNode.Parse(body);
                            username = node["username"]?.ToString();
                            password = node["password"]?.ToString();
                            domainname = node["domain"]?.ToString();
                            Item.Logger.WriteLine($"{username} {password} {domainname}");
                        }
                        break;
                    case "application/x-www-form-urlencoded":
                        Item.Logger.WriteLine("application/x-www-form-urlencoded");
                        using (var reader = new StreamReader(context.Request.Body))
                        {
                            var body = await reader.ReadToEndAsync();

                            foreach (var parameter in body.Split("&"))
                            {
                                var key = parameter.Substring(0, parameter.IndexOf("="));
                                var val = parameter.Substring(parameter.IndexOf("=") + 1);
                                switch (key)
                                {
                                    case "username":
                                        username = val;
                                        break;
                                    case "password":
                                        password = val;
                                        break;
                                    case "domain":
                                        domainname = val;
                                        break;
                                }
                            }
                            Item.Logger.WriteLine($"{username} {password} {domainname}");
                        }
                        break;
                    default:
                        return new
                        {
                            Result = "NG",
                            Description = "Not support Content-Type."
                        };
                }

                var ret_logon = Profile.ConsoleLogon.CheckLogonUser(username, password, domainname);
                if (ret_logon)
                {
                    var ret_agent = Profile.ConsoleLogon.CheckRunningAgent();
                    if (ret_agent)
                    {
                        Profile.ConsoleLogon.Enter(username, password, domainname);
                        return new
                        {
                            Result = "OK",
                            Description = "Logon success."
                        };
                    }
                    else
                    {
                        return new
                        {
                            Result = "NG",
                            Description = "Agent stop."
                        };
                    }
                }
                else
                {
                    return new
                    {
                        Result = "NG",
                        Description = "Failed to logon."
                    };
                }
            });

            //  ユーザーのログオフ
            app.MapGet("/api/user/logoff", () =>
            {
                Item.Logger.WriteLine("[GET]User Logoff.");
                /*
                Item.UserLogonSessionCollection.Sessions = Profile.UserLogonSession.GetLoggedOnSession();
                Item.UserLogonSessionCollection.Sessions.
                    ToList().
                    ForEach(x => x.Logoff());
                return new
                {
                    Result = "OK"
                };*/
            });





            //  ユーザーの切断
            app.MapGet("api/user/disconnect", () =>
            {
                Item.Logger.WriteLine("[GET]User Disconnect, from RDP.");
                return Api.User.Disconnect();
            });
            app.MapPost("api/user/disconnect", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]User Disconnect, from RDP.");
                return Api.User.Disconnect(await UserParameter.SetParamAsync(context));
            });

            //  ログの出力
            app.MapGet("/api/log/print", () =>
            {
                Item.Logger.Pause = true;
                Item.Logger.WriteLine("[GET]Log print.");
                var print = Api.Log.Print();
                Item.Logger.Pause = false;
                return new
                {
                    Log = print,
                };
            });
            app.MapPost("/api/log/print", async (HttpContext context) =>
            {
                Item.Logger.Pause = true;
                Item.Logger.WriteLine("[POST]Log print.");
                var print = Api.Log.Print(await LogParameter.SetParamAsync(context));
                Item.Logger.Pause = false;
                return new
                {
                    Log = print,
                };
            });

            //  マシン情報の取得
            app.MapGet("/api/server/info", () =>
            {
                Item.Logger.WriteLine("[GET]Get System Info.");
                return Api.Server.Info();
            });
            app.MapPost("/api/server/info", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Get System Info.");
                return Api.Server.Info(await ServerParameter.SetParamAsync(context));
            });

            //  サーバのネットワーク情報の取得
            app.MapGet("/api/server/network", () =>
            {
                Item.Logger.WriteLine("[GET]Get Network Info.");
                return new
                {
                    NetworkInterfaces = Api.Server.Network()
                };
            });
            app.MapPost("/api/server/network", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Get Network Info.");
                return new
                {
                    NetworkInterfaces = Api.Server.Network(await ServerParameter.SetParamAsync(context))
                };
            });


            //  サーバのシャットダウン
            app.MapGet("/api/server/close", () =>
            {
                Item.Logger.WriteLine("[GET]Close Application.");
                Api.Server.Close(app);
                return new
                {
                    Result = "OK"
                };
            });
            app.MapPost("/api/server/close", (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Close Application.");
                Api.Server.Close(app);
                return new
                {
                    Result = "OK"
                };
            });
        }
    }
}
