using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    internal class TestResult
    {
        /// <summary>
        /// テスト結果の
        /// - key : Responseの結果からキーの値を取得
        /// - log : ログ取得用のRequestを実行し、その結果から取得
        /// </summary>
        public string TestType { get; set; }

        /// <summary>
        /// テストに投入する文字列,コード,Nodeパス等
        /// </summary>
        public string TestCode { get; set; }

        /// <summary>
        /// テスト時に期待する値
        /// </summary>
        public string Expected { get; set; }

        /// <summary>
        /// テスト実施時の実際の値
        /// </summary>
        //[JsonIgnore]
        //[YamlIgnore]
        public string Actual { get; set; }

        public void SetResponseParameter(ResponseSet responseSet)
        {
            switch (TestType)
            {
                case "key":
                    Actual = GetNodeValue(TestCode, responseSet);
                    break;
                case "log":
                    Actual = GetLogValue(TestCode, responseSet);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 対象の値までのパスからNodeの値を取得する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetNodeValue(string code, ResponseSet responseSet)
        {
            var node = responseSet.Node;
            var pat_index = new Regex(@"\[\d+\]");
            foreach (var field in code.Split("/"))
            {
                if (string.IsNullOrEmpty(field)) continue;
                if (pat_index.IsMatch(field))
                {
                    int index = int.Parse(field.TrimStart('[').TrimEnd(']'));
                    node = node[index];
                }
                else
                {
                    node = node[field];
                }
            }

            return node.ToString();
        }

        /// <summary>
        /// ひとつ前のログから、指定した箇所を取得
        /// 例)
        /// line=3 ⇒ ログの最初の3行だけの文字列。
        /// range=1-5 ⇒ ログの1行目から5行分の文字列
        /// </summary>
        /// <param name="code"></param>
        /// <param name="responseSet"></param>
        /// <returns></returns>
        public string GetLogValue(string code, ResponseSet responseSet)
        {
            responseSet.CheckLastRequestLog();
            if (code.Contains("="))
            {
                var codeVal = code.Split("=");
                switch (codeVal[0])
                {
                    case "line":
                        if (int.TryParse(codeVal[1], out int lineVal))
                        {
                            if (responseSet.StoredLog.Length < lineVal)
                            {
                                lineVal = responseSet.StoredLog.Length;
                            }
                            return string.Join("\n", responseSet.StoredLog.Take(lineVal).Select(x => x.Substring(22)));
                        }
                        break;
                    case "range":
                        var range = codeVal[1].Split("-");
                        if (int.TryParse(range[0], out int rangeVal1) && int.TryParse(range[1], out int rangeVal2) &&
                            responseSet.StoredLog.Length > rangeVal2)
                        {
                            return string.Join("\n", responseSet.StoredLog.Skip(rangeVal1).Take(rangeVal2));
                        }
                        break;
                    default:
                        return "";
                }
            }
            return "";
        }
    }
}
