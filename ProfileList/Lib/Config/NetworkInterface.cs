using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProfileList.Lib.Config
{
    public class NetworkInterface
    {
        public string Name { get; set; }

        public NetworkAddress[] Addresses { get; set; }
        public string[] GatewayAddress { get; set; }
        public string[] DNSServers { get; set; }
        public string MACAddress { get; set; }
        public string MACAddress_alias1 { get; set; }
        public string MACAddress_alias2 { get; set; }
        public string GUID { get; set; }
        public string DeviceName { get; set; }
        public string Manufacturer { get; set; }
        public bool? DHCPEnabled { get; set; }
        public string DHCPServer { get; set; }
        public string[] DNSDomainSuffixSearchOrder { get; set; }

        public NetworkInterface(ManagementObject mo_conf, IEnumerable<ManagementBaseObject> mo_adapters)
        {
            string guid = mo_conf["SettingID"] as string;
            var mo_adapter = mo_adapters.
                FirstOrDefault(mo => (string)mo["GUID"] == guid);

            Name = mo_adapter["NetConnectionID"] as string;
            Addresses = NetworkAddress.GetAddresses(mo_conf);
            GatewayAddress = mo_conf["DefaultIPGateway"] as string[];
            DNSServers = mo_conf["DNSServerSearchOrder"] as string[];
            MACAddress = mo_adapter["MACAddress"] as string;
            MACAddress_alias1 = (mo_adapter["MACAddress"] as string).Replace(":", "-");
            MACAddress_alias2 = (mo_adapter["MACAddress"] as string).Replace(":", "").ToLower();
            GUID = guid;
            DeviceName = mo_adapter["ProductName"] as string;
            Manufacturer = mo_adapter["Manufacturer"] as string;
            DHCPEnabled = bool.TryParse(mo_conf["DHCPEnabled"] as string, out bool b) ? b : null;
            DHCPServer = mo_conf["DHCPServer"] as string;
            DNSDomainSuffixSearchOrder = mo_conf["DNSDomainSuffixSearchOrder"] as string[];
        }
    }
}
