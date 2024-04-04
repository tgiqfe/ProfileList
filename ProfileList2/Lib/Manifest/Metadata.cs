using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace ProfileList2.Lib.Manifest
{
    public class Metadata
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "description")]
        public string Description { get; set; }

        [YamlMember(Alias = "script")]
        public ScriptPath[] Script { get; set; }

        [YamlMember(Alias = "outputType")]
        public OutputType OutputType { get; set; }

        [YamlMember(Alias = "outputFilePath")]
        public string OutputFilePath { get; set; }

        [YamlMember(Alias = "method")]
        public string Method { get; set; }

        /// <summary>
        /// RequestのHttpメソッドからマッチするかどうかをチェック
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool IsMatchMethod(string method)
        {
            return Method.
                Split(",").
                Select(x => x.Trim()).
                Any(x => x.Equals(method, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Scriptパラメータから、OSに応じたスクリプトのパスを取得
        /// </summary>
        /// <returns></returns>
        public string GetScriptPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Script.
                    Where(x => x.OS == OSType.Windows).
                    Select(x => x.Path).
                    FirstOrDefault();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Script.
                    Where(x => x.OS == OSType.Linux).
                    Select(x => x.Path).
                    FirstOrDefault();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Script.
                    Where(x => x.OS == OSType.Mac).
                    Select(x => x.Path).
                    FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
