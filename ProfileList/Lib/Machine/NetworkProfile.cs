
using System.Management;

namespace ProfileList.Lib.Machine
{
    public class NetworkProfile
    {
        public NetworkInterface[] Interfaces { get; set; }

        public NetworkProfile()
        {
            var mo_adapters = new ManagementClass("Win32_NetworkAdapter").
                GetInstances().
                OfType<ManagementObject>();
            this.Interfaces = new ManagementClass("Win32_NetworkAdapterConfiguration").
                GetInstances().
                OfType<ManagementObject>().
                Where(mo => (bool)mo["IPEnabled"]).
                Select(mo => new NetworkInterface(mo, mo_adapters)).
                ToArray();
        }
    }
}
