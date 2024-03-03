using ProfileList.Lib.Machine;
using ProfileList.Lib.Profile;
using System.Diagnostics;

namespace ProfileList
{
    public class Item
    {
        public static string WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public static Setting Setting = null;
        public static MachineInfo MachineInfo = null;
        public static NetworkProfile NetworkProfile = null;
        public static UserLogonSessionCollection UserLogonSessionCollection = null;
        public static UserProfileCollection UserProfileCollection = null;
        public static Logger Logger = null;
    }
}
