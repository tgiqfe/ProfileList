﻿using System;
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

        public void SetResponseParameter(ResponseSet responseSet, string server)
        {
            switch (TestType)
            {
                case "key":
                    Actual = GetNodeValue(TestCode, responseSet.Node);
                    break;
                case "log":
                    Actual = GetLogValue(server);
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
        public string GetNodeValue(string code, JsonNode node)
        {
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

        public string GetLogValue(string server)
        {
            string url = $"{server}/api/log/print";
            var data = new StringContent("{ \"request\": 1 }", Encoding.UTF8, TestAction.CONTENT_TYPE_JSON);
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url, data).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var node = JsonNode.Parse(content,
                    new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
                

                return GetNodeValue(TestCode, node);
            }
        }
    }
}