﻿using ProfileList.Lib.Api;
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
                Item.UserLogonSessions = Profile.UserLogonSession.GetLoggedOnSession();
                return new
                {
                    Sessions = Item.UserLogonSessions,
                };
            });
            app.MapPost("/api/user/session", () =>
            {
                Item.Logger.WriteLine("[POST]Get user logon sessions.");
                Item.UserLogonSessions = Profile.UserLogonSession.GetLoggedOnSession();
                return new
                {
                    Sessions = Item.UserLogonSessions,
                };
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
                Item.UserLogonSessions = Profile.UserLogonSession.GetLoggedOnSession();
                Item.UserLogonSessions.
                    ToList().
                    ForEach(x => x.Logoff());
                return new
                {
                    Result = "OK"
                };
            });

            //  ユーザーの切断
            app.MapGet("api/user/disconnect", () =>
            {
                Item.Logger.WriteLine("[GET]User Disconnect, from RDP.");
                Item.UserLogonSessions = Profile.UserLogonSession.GetLoggedOnSession();
                Item.UserLogonSessions.
                    Where(x => x.ProtocolType == 2).
                    ToList().
                    ForEach(x => x.Disconnect());
                return new
                {
                    Result = "OK"
                };
            });



            //  ログの出力 (無指定の場合は10行)
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








            //  サーバ情報のリフレッシュ
            app.MapGet("/api/server/refresh", () =>
            {
                Item.Logger.WriteLine("[GET]System Refresh.");
                Item.MachineInfo = new();
                Item.UserProfileCollection = new();
                return new
                {
                    Result = "OK"
                };
            });

            //  サーバ情報の取得
            app.MapGet("/api/server/info", () =>
            {
                Item.Logger.WriteLine("[GET]Get System Info.");
                return new
                {
                    Item.MachineInfo,
                };
            });

            //  サーバのシャットダウン
            app.MapGet("/api/server/close", () =>
            {
                Item.Logger.WriteLine("[GET]Close Application.");
                Task.Run(() =>
                {
                    Task.Delay(1000);
                    app.StopAsync();
                });
                return new
                {
                    Result = "OK"
                };
            });
        }
    }
}
