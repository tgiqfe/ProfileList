using System.Management;
using System.Text.Json;

namespace ProfileList.Lib.Profile
{
    public class UserProfileCollection
    {
        public UserProfile[] Profiles { get; set; }

        public UserProfileCollection()
        {
            this.Profiles = new ManagementClass("Win32_UserProfile").
                GetInstances().
                OfType<ManagementObject>().
                Where(x =>
                {
                    var sid = x["SID"] as string;
                    return !string.IsNullOrEmpty(sid) &&
                        Item.MachineInfo.SystemSIDs.All(y => y != sid);
                }).
                Select(x => new UserProfile(x)).
                ToArray();
        }

        /// <summary>
        /// ユーザープロファイルの情報を保存
        /// </summary>
        public void Save()
        {
            string outputPath = "userprofiles.json";
            File.WriteAllText(outputPath,
                JsonSerializer.Serialize(this.Profiles,
                    new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    }));
        }
    }
}
