using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Lib;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    internal class TestFileCollection
    {
        private const string PARENT_NAME = "TestCases";

        public List<TestFile> Files { get; set; }

        /// <summary>
        /// マニフェストファイルを詠み込んでセット
        /// </summary>
        public TestFileCollection()
        {
            this.Files = new();
            string parent = Path.Combine(Item.WorkingDirectory, PARENT_NAME);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            foreach (string path in Directory.GetFiles(parent, "*.yml"))
            {
                TestFile file = null;
                try
                {
                    string content = File.ReadAllText(path);
                    file = new Deserializer().Deserialize<TestFile>(content);
                }
                catch { }
                file ??= new TestFile();
                file.FileName = Path.GetFileName(path);
                this.Files.Add(file);
            }
        }

        /// <summary>
        /// マニフェストファイルを保存
        /// </summary>
        public void Save()
        {
            foreach (var file in this.Files)
            {
                string path = Path.Combine(Item.WorkingDirectory, PARENT_NAME, file.FileName);
                string content = new SerializerBuilder().
                    WithEventEmitter(x => new MultilineScalarFlowStyleEmitter(x)).
                    WithEmissionPhaseObjectGraphVisitor(x => new YamlIEnumerableSkipEmptyObjectGraphVisitor(x.InnerVisitor)).
                    Build().
                    Serialize(file);
                File.WriteAllText(path, content);
            }
        }
    }
}
