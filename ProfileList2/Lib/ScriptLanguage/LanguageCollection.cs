using System.Diagnostics;
using System.Text.Json;
using System.Text;
using YamlDotNet.Serialization;
using ProfileList2.Lib.ScriptLanguage.Yml;

namespace ProfileList2.Lib.ScriptLanguage
{
    public class LanguageCollection
    {
        public List<Language> Languages { get; set; }

        #region Load/Save

        public static LanguageCollection Load(string path)
        {
            LanguageCollection collection = null;
            try
            {
                var content = File.ReadAllText(path, Encoding.UTF8);
                collection = Path.GetExtension(path) switch
                {
                    ".json" => JsonSerializer.Deserialize<LanguageCollection>(content),
                    ".yaml" or ".yml" => new Deserializer().Deserialize<LanguageCollection>(content),
                    _ => null,
                };
            }
            catch { }
            collection ??= new LanguageCollection() { Languages = DefaultLanguageSetting.Generate() };
            return collection;
        }

        public void Save(string path)
        {
            var content = Path.GetExtension(path) switch
            {
                ".json" => JsonSerializer.Serialize(this,
                    new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        IgnoreReadOnlyProperties = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true,
                        //Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase) },
                    }),
                ".yaml" or ".yml" => new SerializerBuilder().
                    WithEventEmitter(x => new MultilineScalarFlowStyleEmitter(x)).
                    WithEmissionPhaseObjectGraphVisitor(x => new YamlIEnumerableSkipEmptyObjectGraphVisitor(x.InnerVisitor)).
                    Build().
                    Serialize(this),
                _ => null,
            };
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        #endregion

        public Language GetLanguage(string keyword)
        {
            if (File.Exists(keyword))
            {
                string extension = Path.GetExtension(keyword);
                return this.Languages.FirstOrDefault(x =>
                    x.Extensions.Any(y =>
                        y.Equals(extension, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                return this.Languages.FirstOrDefault(x =>
                    x.Name.Equals(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Alias?.Any(y => y.Equals(keyword, StringComparison.OrdinalIgnoreCase)) ?? false));
            }
        }

        public Process GetProcess(string path, string arguments = "")
        {
            if (File.Exists(path))
            {
                string extension = Path.GetExtension(path);
                var lang = this.Languages.FirstOrDefault(x =>
                    x.Extensions.Any(y =>
                        y.Equals(extension, StringComparison.OrdinalIgnoreCase)));
                return lang?.GetProcess(path, arguments);
            }
            return null;
        }
    }
}
