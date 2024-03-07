using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    internal class ResponseSet
    {
        public HttpResponseMessage Response { get; set; }

        public string Content { get; set; }

        public JsonNode Node { get; set; }

        public string Server { get; set; }

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
    }
}
