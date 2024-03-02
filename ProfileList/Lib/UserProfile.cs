using Microsoft.AspNetCore.Mvc.Razor;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace ProfileList.Lib
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
            this.Extension = mo;
            this.SID = mo["SID"] as string;
            this.ProfilePath = mo["LocalPath"] as string;

            var uamo = new ManagementClass("Win32_UserAccount").
                GetInstances().
                OfType<ManagementObject>().
                Where(x => x["SID"] as string == this.SID).
                FirstOrDefault(x => x["SID"] as string == this.SID);
            if (uamo == null)
            {
                this.UserName = "-";
                this.Caption = "不明なアカウント";
            }
            else
            {
                this.UserName = uamo["Name"] as string;
                this.Caption = uamo["Caption"] as string;
                this.IsDomainUser = !(bool)uamo["LocalAccount"];
                this.UserDomain = uamo["Domain"] as string;
                this.IsLogon = Item.MachineInfo.UserLogonSessions.
                    FirstOrDefault(x => x.UserName == this.UserName && x.UserDomain == this.UserDomain)?.IsActive() ?? false;
                this.FileSystemCount = new FileSystemCount(this.ProfilePath, true);
            }
        }
    }
}
