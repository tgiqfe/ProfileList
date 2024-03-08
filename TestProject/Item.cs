using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    internal class Item
    {
        public static string WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    }
}
