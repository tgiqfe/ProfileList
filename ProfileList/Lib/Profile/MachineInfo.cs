using System.Management;

namespace ProfileList.Lib.Profile
{
    public class MachineInfo
    {
        public string ComputerName { get; private set; }
        public string DomainName { get; private set; }
        public bool IsDomainMachine { get; private set; }
        public string[] SystemSIDs { get; private set; }

        public MachineInfo()
        {
            ComputerName = Environment.MachineName;
            DomainName = new ManagementClass("Win32_ComputerSystem").
                GetInstances().
                OfType<ManagementObject>().
                First()["Domain"] as string;
            IsDomainMachine = (bool)new ManagementClass("Win32_ComputerSystem").
                GetInstances().
                OfType<ManagementObject>().
                First()["PartOfDomain"];
            SystemSIDs = new ManagementClass("Win32_SystemAccount").
                GetInstances().
                OfType<ManagementObject>().
                Select(x => x["SID"] as string).
                ToArray();
        }
    }
}
