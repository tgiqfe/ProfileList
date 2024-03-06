using ProfileList.Lib.Config;
using ProfileList.Lib.Profile;
using System.Diagnostics;

namespace ProfileList
{
    public class Item
    {
        /// <summary>
        /// アプリケーション起動ディレクトリ
        /// </summary>
        public static string WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        /// <summary>
        /// setting.jsonを読み込んだデータ
        /// </summary>
        public static Setting Setting = null;

        /// <summary>
        /// マシン情報
        /// </summary>
        public static MachineInfo MachineInfo = null;

        /// <summary>
        /// ネットワーク情報
        /// </summary>
        public static NetworkProfile NetworkProfile = null;
        
        /// <summary>
        /// 現在ログオンしているユーザーのセッション情報
        /// </summary>
        public static UserLogonSessionCollection UserLogonSessionCollection = null;

        /// <summary>
        /// ログオンしたことのあるユーザープロファイルの情報
        /// </summary>
        public static UserProfileCollection UserProfileCollection = null;

        /// <summary>
        /// ログ出力用のインスタンス
        /// </summary>
        public static Logger Logger = null;
    }
}
