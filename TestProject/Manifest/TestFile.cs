using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    internal class TestFile
    {
        public TestCaseSetting TestCase { get; set; }

        [JsonIgnore]
        [YamlIgnore]
        public string FileName { get; set; }
    }
}
