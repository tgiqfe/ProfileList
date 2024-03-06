using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Manifest
{
    public class TestCase
    {
        public string Server_Protocol { get; set; }
        public string Server_Address { get; set; }
        public int Server_Port { get; set; }
        public string Description { get; set; }

        public List<TestAction> ActionList { get; set; }

    }
}
