using System.Text;
using YamlDotNet.Serialization;

namespace ProfileList2.Lib
{
    public class MetadataFile
    {
        [YamlMember(Alias = "metadata")]
        public Metadata Metadata { get; set; }

        public static MetadataFile Load(string path)
        {
            MetadataFile mdFile = null;
            try
            {
                var yml = File.ReadAllText(path, Encoding.UTF8);
                mdFile = new Deserializer().Deserialize<MetadataFile>(yml);
            }
            catch
            {
                var data = new MetadataFile()
                {
                    Metadata = new Metadata()
                    {
                        Name = "Unknown",
                        Description = "Unknown",
                        ScriptPath = "Unknown",
                        OutputType = OutputType.Console,
                        OutputFilePath = "Unknown",
                    }
                };
                string content = new Serializer().Serialize(data);
                File.WriteAllText(path, content, Encoding.UTF8);
            }
            mdFile ??= new();
            return mdFile;
        }
    }
}
