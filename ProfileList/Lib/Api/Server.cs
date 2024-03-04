using ProfileList.Lib.Config;

namespace ProfileList.Lib.Api
{
    public class Server
    {

        /// <summary>
        /// マシン情報を取得
        /// </summary>
        /// <returns></returns>
        public static MachineInfo Info(ServerParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, MachineInfo.");
                Item.MachineInfo = new MachineInfo();
            }
            return Item.MachineInfo;
        }

        /// <summary>
        /// サーバのネットワーク情報の取得
        /// 無指定の場合は、メインで使用していると思われる1つだけを返す。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static NetworkInterface[] Network(ServerParameter parameter = null)
        {
            if (parameter?.Refresh == true)
            {
                Item.Logger.WriteLine("Refrsh, NetworkInfo.");
                Item.NetworkProfile = new();
            }
            if (parameter?.All == true)
            {
                Item.Logger.WriteLine("Get All Network interface info.");
                return Item.NetworkProfile.Interfaces;
            }
            else
            {
                //  デフォルトゲーウェイが設定されている = メインで使用中と判断
                Item.Logger.WriteLine("Get main network info.");
                var iface = Item.NetworkProfile.Interfaces.
                        FirstOrDefault(x => x.GatewayAddress?.Length > 0);
                if (iface == null)
                {
                    iface = Item.NetworkProfile.Interfaces[0];
                }
                return new NetworkInterface[] { iface };
            }
        }

        /// <summary>
        /// サーバのシャットダウン
        /// </summary>
        /// <param name="app"></param>
        public static void Close(WebApplication app)
        {
            Item.Logger.WriteLine("Close Service.");
            Task.Run(() =>
            {
                Task.Delay(1000);
                app.StopAsync();
            });
        }
    }
}
