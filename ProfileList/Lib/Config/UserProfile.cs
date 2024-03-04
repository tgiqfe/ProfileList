using System.Management;
using System.Text.Json.Serialization;

namespace ProfileList.Lib.Profile
{
    public class UserProfile
    {
        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public string Caption { get; set; }
        public string ProfilePath { get; set; }
        public string SID { get; set; }
        public bool IsLogon { get; set; }
        public bool IsDomainUser { get; set; }
        public FileSystemCount FileSystemCount { get; set; }

        [JsonIgnore]
        public ManagementObject Extension { get; set; }

        public UserProfile(ManagementObject mo)
        {
            Extension = mo;
            SID = mo["SID"] as string;
            ProfilePath = mo["LocalPath"] as string;

            var uamo = new ManagementClass("Win32_UserAccount").
                GetInstances().
                OfType<ManagementObject>().
                Where(x => x["SID"] as string == SID).
                FirstOrDefault(x => x["SID"] as string == SID);
            if (uamo == null)
            {
                UserName = "-";
                Caption = "不明なアカウント";
            }
            else
            {
                UserName = uamo["Name"] as string;
                Caption = uamo["Caption"] as string;
                IsDomainUser = !(bool)uamo["LocalAccount"];
                UserDomain = uamo["Domain"] as string;
                IsLogon = Item.UserLogonSessionCollection.Sessions.
                    FirstOrDefault(x => x.UserName == UserName && x.UserDomain == UserDomain)?.IsActive() ?? false;
                FileSystemCount = new FileSystemCount(ProfilePath, true);
            }
        }
    }
}
