using System.Reflection;
using System.Text.Json;

namespace ProfileList2
{
    public class Setting
    {
        const string settingFile = "setting.json";

        /// <summary>
        /// 待ち受けポート
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// プロセス実行中に環境変数に保存する情報
        /// </summary>
        public SettingEnvironment Env { get; set; }

        /// <summary>
        /// [PL_ProtectedProfile]パラメータを配列にして取得
        /// </summary>
        public string[] ProtectedProfileUsers
        {
            get
            {
                return this.Env.PL_ProtectedProfile.Split(",").Select(x => x.Trim()).ToArray();
            }
        }

        /// <summary>
        /// 初期パラメータをセット
        /// </summary>
        public void Init()
        {
            this.Port = 5000;
            this.Env = new SettingEnvironment
            {
                PL_RLAgentPipeKey = "____pipe____key____",
                PL_RLAgentMutexKey = "Global\\____mutex____key____",
                PL_ProtectedProfile = "Administrator, Guest, DefaultAccount, Admin, setup",
            };
            Save();
        }

        /// <summary>
        /// 設定ファイルからデシリアライズ
        /// </summary>
        /// <returns></returns>
        public static Setting Load()
        {
            Setting setting = null;
            string outputPath = Path.Combine(Item.WorkingDirectory, settingFile);
            try
            {
                setting = JsonSerializer.Deserialize<Setting>(File.ReadAllText(outputPath));
            }
            catch { }
            if (setting == null)
            {
                setting = new();
                setting.Init();
            }
            return setting;
        }

        /// <summary>
        /// シリアライズして設定ファイルに保存
        /// </summary>
        public void Save()
        {
            string json = JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });
            File.WriteAllText(Path.Combine(Item.WorkingDirectory, settingFile), json);
        }

        /// <summary>
        /// Envパラメータ内の値を環境変数に登録
        /// </summary>
        public void RegisterEnvironment()
        {
            var props = typeof(SettingEnvironment).GetProperties(
                BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.GetType() == typeof(string))
                {
                    Environment.SetEnvironmentVariable(
                        prop.Name,
                        prop.GetValue(this.Env).ToString(),
                        EnvironmentVariableTarget.Process);
                }
            }
        }
    }

    /// <summary>
    /// 環境変数に登録する情報
    /// </summary>
    public class SettingEnvironment
    {
        public string PL_RLAgentPipeKey { get; set; }
        public string PL_RLAgentMutexKey { get; set; }
        public string PL_ProtectedProfile { get; set; }
    }
}
