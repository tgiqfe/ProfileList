using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;

namespace TestProject.Manifest
{
    /// <summary>
    /// テスト動作に対する結果を格納
    /// </summary>
    internal class ResponseSet
    {
        #region public parameter

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
        /// テスト左記サーバのアドレス (例: /api/hello)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// テスト動作1つに対して、ログを取得して格納
        /// </summary>
        public string[] StoredLog { get; set; }

        #endregion

        public ResponseSet() { }
        public ResponseSet(string server, string address)
        {
            this.Server = server;
            this.Address = address;
        }

        /// <summary>
        /// Requestを送信して、レスポンスを格納
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="address"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(HttpClient client, string method, StringContent data)
        {
            string url = $"{this.Server}{this.Address}";
            this.Response = method switch
            {
                TestAction.METHOD_GET => await client.GetAsync(url),
                TestAction.METHOD_POST => await client.PostAsync(url, data),
                TestAction.METHOD_PUT => await client.PutAsync(url, data),
                TestAction.METHOD_DELETE => await client.SendAsync(new HttpRequestMessage()
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = data
                }),
                _ => null
            };
            using (var ms = new MemoryStream())
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(this.Response.Content.ReadAsStream(), XmlDictionaryReaderQuotas.Max))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, true, true))
            {
                writer.WriteNode(reader, true);
                writer.Flush();
                this.Content = Encoding.UTF8.GetString(ms.ToArray());
            }
            this.Node = JsonNode.Parse(
                this.Content,
                new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
        }

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
    }
}
