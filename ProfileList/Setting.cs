using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProfileList
{
    public class Setting
    {
        const string settingFile = "setting.json";

        public int Port { get; set; }
        public string RLAgentPipeKey { get; set; }
        public string RLAgentMutexKey { get; set; }
        public string LogDirectory { get; set; }
        public string ProtectedProfile { get; set; }

        [JsonIgnore]
        public string[] ProtectedProfileUsers
        {
            get
            {
                return ProtectedProfile.Split(",").Select(x => x.Trim()).ToArray();
            }
        }

        public void Init()
        {
            this.Port = 5000;
            this.RLAgentPipeKey = "____pipe____key____";
            this.RLAgentMutexKey = "Global\\____mutex____key____";
            this.LogDirectory = "Logs";
            this.ProtectedProfile = "Administrator, Guest, DefaultAccount, Admin, setup";
            Save();
        }

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

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(Path.Combine(Item.WorkingDirectory, settingFile), json);
        }


    }
}
