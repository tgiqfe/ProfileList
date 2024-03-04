using System.Management;
using System.Text.Json.Serialization;

namespace ProfileList.Lib.Config
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

        public string IPAddress { get; private set; }
        public string SubnetMask { get; private set; }
        public string DefaultGateway { get; private set; }
        public string DNSServers { get; private set; }

        #endregion

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

            if (Item.NetworkProfile == null)
            {
                var mo_conves = new ManagementClass("Win32_NetworkAdapterConfiguration").
                    GetInstances().
                    OfType<ManagementObject>().
                    Where(x => (bool)x["IPEnabled"]).
                    ToArray();
                var nwConf = mo_conves.
                    FirstOrDefault(x => !string.IsNullOrEmpty(x["DefaultIPGateway"] as string));
                if (nwConf == null)
                {
                    nwConf = mo_conves[0];
                }
                IPAddress = (nwConf["IPAddress"] as string[])[0];
                SubnetMask = (nwConf["IPSubnet"] as string[])[0];
                DefaultGateway = (nwConf["DefaultIPGateway"] as string[])[0];
                DNSServers = string.Join(", ", nwConf["DNSServerSearchOrder"] as string[]);
            }
            else
            {
                var iface = Item.NetworkProfile.Interfaces.
                    FirstOrDefault(x => x.GatewayAddress?.Length > 0);
                if (iface == null)
                {
                    iface = Item.NetworkProfile.Interfaces[0];
                }
                IPAddress = iface.Addresses[0].IPAddress;
                SubnetMask = iface.Addresses[0].SubnetMask;
                DefaultGateway = iface.GatewayAddress?.Length > 0 ? iface.GatewayAddress[0] : "";
                DNSServers = string.Join(", ", iface.DNSServers);
            }
        }
    }
}
