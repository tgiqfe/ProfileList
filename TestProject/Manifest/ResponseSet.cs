using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    /// <summary>
    /// テスト動作に対する結果を格納
    /// </summary>
    internal class ResponseSet
    {
        /// <summary>
        /// レスポンス
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// レスポンスから読み込んだJSON文字列
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// レスポンスから読み込んだJSON文字列をパースしたNode
        /// </summary>
        public JsonNode Node { get; set; }

        /// <summary>
        /// テスト先のサーバのアドレス (例: http://localhost:5000)
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// テスト動作1つに対して、ログを取得して格納
        /// </summary>
        public string[] StoredLog { get; set; }

        /// <summary>
        /// /api/log/print で、、request=1のリクエストを送信して、
        /// 最後のリクエストログを取得して格納。
        /// </summary>
        public void CheckLastRequestLog()
        {
            if (this.StoredLog == null)
            {
                string url = $"{this.Server}/api/log/print";
                using (var data = new StringContent("{ \"request\": 1 }", Encoding.UTF8, TestAction.CONTENT_TYPE_JSON))
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(url, data).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    var node = JsonNode.Parse(content,
                        new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
                    this.StoredLog = node["log"].AsArray().Select(x => x.ToString()).ToArray();
                }
            }
        }

        public async Task SendGetAsync(HttpClient client, string url)
        {
            this.Response = await client.GetAsync(url);
            SetContent();
            SetNode();
        }

        public async Task SendPostAsync(HttpClient client, string url, StringContent data)
        {
            this.Response = await client.PostAsync(url, data);
            SetContent();
            SetNode();
        }

        public async Task SendPutAsync(HttpClient client, string url, StringContent data)
        {
            this.Response = await client.PutAsync(url, data);
            SetContent();
            SetNode();
        }

        public async Task SendDeleteAsync(HttpClient client, string url, StringContent data)
        {
            this.Response = await client.SendAsync(new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = data
            });
            SetContent();
            SetNode();
        }

        public void SetContent()
        {
            using (var ms = new MemoryStream())
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(this.Response.Content.ReadAsStream(), XmlDictionaryReaderQuotas.Max))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, true, true))
            {
                writer.WriteNode(reader, true);
                writer.Flush();
                this.Content = Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public void SetNode()
        {
            this.Node = JsonNode.Parse(
                this.Content,
                new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
