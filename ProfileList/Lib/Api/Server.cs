using ProfileList.Lib.Machine;

namespace ProfileList.Lib.Api
{
    public class Server
    {

        /// <summary>
        /// サーバの情報を取得
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
