using System.Management;
using System.Text.Json;

namespace ProfileList.Lib
{
    public class UserProfileCollection
    {
        public UserProfile[] UserProfiles { get; set; }

        public UserProfileCollection()
        {
            this.UserProfiles = new ManagementClass("Win32_UserProfile").
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

        public void Save()
        {
            string outputPath = "userprofiles.json";
            File.WriteAllText(outputPath,
                JsonSerializer.Serialize(this.UserProfiles,
                    new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    }));
        }
    }
}
