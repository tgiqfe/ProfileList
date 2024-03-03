using System.Management;
using System.Text.Json.Serialization;

namespace ProfileList.Lib.Machine
{
    public class MachineInfo
    {
        #region Public parameter

        public string ComputerName { get; private set; }
        public string DomainName { get; private set; }
        public bool IsDomainMachine { get; private set; }

        [JsonIgnore]
        public string[] SystemSIDs { get; private set; }
        [JsonPropertyName("SystemSIDs")]
        public string SystemSIDsString
        {
            get
            {
                return string.Join(", ", SystemSIDs);
            }
        }

        public NetworkProfile NetworkProfile { get; private set; }

        #endregion

        public MachineInfo()
        {
            this.ComputerName = Environment.MachineName;
            this.DomainName = new ManagementClass("Win32_ComputerSystem").
                GetInstances().
                OfType<ManagementObject>().
                First()["Domain"] as string;
            this.IsDomainMachine = (bool)new ManagementClass("Win32_ComputerSystem").
                GetInstances().
                OfType<ManagementObject>().
                First()["PartOfDomain"];
            this.SystemSIDs = new ManagementClass("Win32_SystemAccount").
                GetInstances().
                OfType<ManagementObject>().
                Select(x => x["SID"] as string).
                ToArray();
            this.NetworkProfile = new();
        }
    }
}
