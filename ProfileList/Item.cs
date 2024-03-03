using ProfileList.Lib;
using System.Diagnostics;

namespace ProfileList
{
    public class Item
    {
        public static string WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public static Setting Setting = null;
        public static MachineInfo MachineInfo = null;
        public static IEnumerable<UserLogonSession> UserLogonSessions = null;
        public static UserProfileCollection UserProfileCollection = null;
        public static Logger Logger = null;
    }
}
