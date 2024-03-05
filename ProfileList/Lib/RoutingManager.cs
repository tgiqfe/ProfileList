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
            app.MapGet("/api/profile/list", () =>
            {
                Item.Logger.WriteLine("[GET]Get user profile list.");
                return Api.Profile.List();
            });
            app.MapPost("/api/profile/list", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Get user profile list.");
                return Api.Profile.List(await ApiParameter.SetAsync<ProfileParameter>(context));
            });

            //  プロファイルを削除
            app.MapPost("/api/profile/delete", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]Delete user profile.");
                return Api.Profile.Delete(await ApiParameter.SetAsync<ProfileParameter>(context));
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
                return Api.User.Session(await ApiParameter.SetAsync<UserParameter>(context));
            });

            //  ユーザーのログオン
            app.MapPost("/api/user/logon", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]User Logon.");
                return Api.User.Logon(await ApiParameter.SetAsync<UserParameter>(context));
            });

            //  ユーザーのログオフ
            app.MapGet("/api/user/logoff", () =>
            {
                Item.Logger.WriteLine("[GET]User Logoff.");
                return Api.User.Logoff();
            });
            app.MapPost("/api/user/logoff", async (HttpContext context) =>
            {
                Item.Logger.WriteLine("[POST]User Logoff.");
                return Api.User.Logoff(await ApiParameter.SetAsync<UserParameter>(context));
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
                return Api.User.Disconnect(await ApiParameter.SetAsync<UserParameter>(context));
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
                var print = Api.Log.Print(await ApiParameter.SetAsync<LogParameter>(context));
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
                return Api.Server.Info(await ApiParameter.SetAsync<ServerParameter>(context));
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
                    networkInterface = Api.Server.Network(await ApiParameter.SetAsync<ServerParameter>(context))
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
