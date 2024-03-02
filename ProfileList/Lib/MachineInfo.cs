using System.Management;

namespace ProfileList.Lib
{
    public class MachineInfo
    {
        public string ComputerName { get; private set; }
        public string DomainName { get; private set; }
        public bool IsDomainMachine { get; private set; }   
        public string[] SystemSIDs { get; private set; }
        public IEnumerable<UserLogonSession> UserLogonSessions { get; private set; }

        public MachineInfo()
        {
            this.ComputerName = Environment.MachineName;
            this.DomainName = (new ManagementClass("Win32_ComputerSystem").
                GetInstances().
                OfType<ManagementObject>().
                First())["Domain"] as string;
            this.IsDomainMachine = (bool)(new ManagementClass("Win32_ComputerSystem").
                GetInstances().
                OfType<ManagementObject>().
                First())["PartOfDomain"];
            this.SystemSIDs = new ManagementClass("Win32_SystemAccount").
                GetInstances().
                OfType<ManagementObject>().
                Select(x => x["SID"] as string).
                ToArray();
            this.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
        }

        public void RefreshSessionInfo()
        {
            this.UserLogonSessions = UserLogonSession.GetLoggedOnSession();
        }
    }
}
