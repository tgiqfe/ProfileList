using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using TestProject.Lib;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    internal class TestCaseSetting
    {
        public TestCase TestCase { get; set; }

        public static TestCaseSetting Load()
        {
            string path = Path.Combine(Item.WorkingDirectory, "testcase.yml");
            TestCaseSetting tc = null;
            try
            {
                string content = File.ReadAllText(path);
                tc = new Deserializer().Deserialize<TestCaseSetting>(content);
            }
            catch { }
            if(tc == null)
            {
                tc = new();
                tc.Init();
            }
            return tc;
        }

        public void Init()
        {
        }

        public void Save()
        {
            string path = Path.Combine(Item.WorkingDirectory, "testcase.yml");

            var builder = new SerializerBuilder().
                WithEventEmitter(x => new MultilineScalarFlowStyleEmitter(x)).
                Build();
            string content = builder.Serialize(this);

            
            //string content = new Serializer().Serialize(this);
            File.WriteAllText(path, content);
        }
    }
}
