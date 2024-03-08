using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using TestProject.Lib;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    /// <summary>
    /// テストケースのTopクラス
    /// </summary>
    internal class TestCaseSetting
    {
        //private const string PARENT_NAME = "TestCases";

        /// <summary>
        /// テストケースを格納するリスト
        /// </summary>
        public List<TestCase> List { get; set; } = new();

        public TestCaseSetting()
        {
            /*
            this.TestCaseList = new();
            string parent = Path.Combine(Item.WorkingDirectory, PARENT_NAME);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            foreach (string path in Directory.GetFiles(parent, "*.yml"))
            {
                TestCase tc = null;
                try
                {
                    string content = File.ReadAllText(path);
                    tc = new Deserializer().Deserialize<TestCase>(content);
                }
                catch { }
                tc ??= new TestCase();
                tc.FileName = Path.GetFileName(path);
                TestCaseList.Add(tc);
            }
            */
        }

        /*
        /// <summary>
        /// 各TestCaseをファイルに保存
        /// </summary>
        public void Save()
        {
            foreach (var tc in this.List)
            {
                string path = Path.Combine(Item.WorkingDirectory, PARENT_NAME, tc.FileName);
                string content = new SerializerBuilder().
                    WithEventEmitter(x => new MultilineScalarFlowStyleEmitter(x)).
                    Build().Serialize(this);
                File.WriteAllText(path, content);
            }
        }
        */
    }
}
