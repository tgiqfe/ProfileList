using System.Diagnostics;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace ProfileList2.Lib.ScriptLanguage
{
    public class Language
    {
        /// <summary>
        /// スクリプトを実行させる言語名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 別名
        /// </summary>
        public string[] Alias { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 関連する拡張子
        /// </summary>
        public string[] Extensions { get; set; }

        /// <summary>
        /// 文字コード
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// x64用コマンド
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// x86用コマンド
        /// </summary>
        public string Command_x86 { get; set; }

        /// <summary>
        /// Processに渡す引数の前に付ける文字列
        /// </summary>
        public string ArgsPrefix { get; set; }

        /// <summary>
        /// Processに渡す引数のスクリプトファイルの直後に付ける文字列
        /// </summary>
        public string ArgsMidWithoutArgs { get; set; }

        /// <summary>
        /// Processに渡す引数のスクリプトファイルの直後に付ける文字列
        /// </summary>
        public string ArgsMidWithArgs { get; set; }

        /// <summary>
        /// Processに渡す引数の最後に付ける文字列
        /// </summary>
        public string ArgsSuffix { get; set; }

        [JsonIgnore]
        [YamlIgnore]
        public string ExtensionText
        {
            get { return string.Join(" ", this.Extensions); }
        }

        public Language() { }

        public override string ToString()
        {
            return $"{this.Name}[{this.ExtensionText}]";
        }

        public Process GetProcess(string scriptFile, string arguments)
        {
            if (this.Command == null)
            {
                return new()
                {
                    StartInfo = new()
                    {
                        FileName = scriptFile,
                        Arguments = arguments,
                    }
                };
            }
            else
            {
                return new()
                {
                    StartInfo = new()
                    {
                        FileName = Environment.Is64BitOperatingSystem || string.IsNullOrEmpty(this.Command_x86) ?
                            this.Command : Command_x86,
                        Arguments = string.IsNullOrEmpty(arguments) ?
                            $"{this.ArgsPrefix}{scriptFile}{this.ArgsMidWithoutArgs}" :
                            $"{this.ArgsPrefix}{scriptFile}{this.ArgsMidWithArgs}{arguments}{this.ArgsSuffix}",
                    }
                };
            }
        }
    }
}
