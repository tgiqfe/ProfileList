using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProfileList.Lib.Machine
{
    public class NetworkAddress
    {
        public string IPAddress { get; private set; }
        public string SubnetMask { get; private set; }
        public int Prefixlength { get; private set; }
        public string IPAddress_alias { get; private set; }

        public NetworkAddress() { }
        public NetworkAddress(string ip, string sm)
        {
            this.IPAddress = ip;
            this.SubnetMask = sm;
            this.Prefixlength = GetPrefixLength(sm);
            this.IPAddress_alias = $"{IPAddress}/{Prefixlength}";
        }

        public System.Net.IPAddress GetIPAddress()
        {
            return System.Net.IPAddress.TryParse(IPAddress, out var ip) ? ip : null;
        }

        public System.Net.IPAddress GetSubnetMask()
        {
            return System.Net.IPAddress.TryParse(SubnetMask, out var sm) ? sm : null;
        }

        public static NetworkAddress[] GetAddresses(ManagementObject mo_conf)
        {
            List<NetworkAddress> list = new();

            var ipaddresses = mo_conf["IPAddress"] as string[];
            var subnetmasks = mo_conf["IPSubnet"] as string[];

            int count = Math.Min(ipaddresses.Length, subnetmasks.Length);
            for (int i = 0; i < count; i++)
            {
                list.Add(new NetworkAddress(ipaddresses[i], subnetmasks[i]));
            }

            return list.ToArray();
        }

        /// <summary>
        /// サブネットマスクからプレフィックス長を取得
        /// </summary>
        /// <param name="subnetmask"></param>
        /// <returns></returns>
        public static int GetPrefixLength(string subnetmask)
        {
            if (System.Net.IPAddress.TryParse(subnetmask, out var sm))
            {
                byte[] bytes = sm.GetAddressBytes();
                int count = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((bytes[i] & (1 << j)) != 0)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
            return -1;
        }
    }
}
