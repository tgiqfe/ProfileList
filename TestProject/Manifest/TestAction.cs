using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    public class TestAction
    {
        #region private static parameter

        private const string CONTENT_TYPE_FORM = "application/x-www-form-urlencoded";
        private const string CONTENT_TYPE_JSON = "application/json";
        private const string METHOD_GET = "GET";
        private const string METHOD_POST = "POST";
        private const string METHOD_PUT = "PUT";
        private const string METHOD_DELETE = "DELETE";

        #endregion

        private string _server_URL = null;
        private int _server_Port = 0;

        public string Address { get; set; }

        /// <summary>
        /// Httpリクエストのメソッド
        /// GET, POST, PUT, DELETEの4つのみ
        /// </summary>
        private string _method = METHOD_GET;
        public string Method
        {
            get { return _method; }
            set
            {
                _method = value.ToLower() switch
                {
                    "get" => METHOD_GET,
                    "post" => METHOD_POST,
                    "put" => METHOD_PUT,
                    "delete" => METHOD_DELETE,
                    _ => METHOD_GET
                };
            }
        }

        /// <summary>
        /// HttpリクエストのContent-Type
        /// application/jsonとapplication/x-www-form-urlencodedのみ
        /// </summary>
        private readonly static string[] _validContentTypes = new string[] { CONTENT_TYPE_FORM, CONTENT_TYPE_JSON };
        private string _contentType = null;
        public string ContentType
        {
            get
            {
                if (_contentType == null && this.BodpyParameters?.Count > 0)
                {
                    _contentType = CONTENT_TYPE_FORM;
                }
                return _contentType;
            }
            set
            {
                _contentType = _validContentTypes.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase)) ? value : "";
            }
        }

        public Dictionary<string, string> BodpyParameters { get; set; }

        [JsonIgnore]
        [YamlIgnore]
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// curlコマンドを生成する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string toCurlCommand(string url)
        {
            StringBuilder sb = new();
            sb.Append("curl");
            sb.Append($" -X {this.Method}");
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                sb.Append($" -H \"Content-Type: {this.ContentType}\"");
            }
            sb.Append($" {url}");
            if (this.BodpyParameters != null)
            {
                //  -d "aaaa=b" -d "bbbb=c" -d "cccc=d" の記述にする場合
                foreach (var pair in this.BodpyParameters)
                {
                    sb.Append($" -d \"{pair.Key}={pair.Value}\"");
                }
                //  -d aaaa=b&bbbb=c&cccc=d の記述にする場合
                //string data = " -d \"" + string.Join("&", this.BodpyParameters.Select(x => $"{x.Key}={x.Value}")) + "\"";
                //sb.Append(data);
            }

            return sb.ToString();
        }

        public string GetBodyData()
        {
            return this.ContentType switch
            {
                CONTENT_TYPE_FORM => string.Join("&", this.BodpyParameters.Select(x => $"{x.Key}={x.Value}")),
                CONTENT_TYPE_JSON => JsonSerializer.Serialize(this.BodpyParameters),
                _ => ""
            };
        }

        public async Task Send(string url)
        {
            var data = new StringContent(GetBodyData(), Encoding.UTF8, this.ContentType);
            var client = new HttpClient();
            this.Response = this.Method switch
            {
                METHOD_GET => await client.GetAsync(url),
                METHOD_POST => await client.PostAsync(url, data),
                METHOD_PUT => await client.PutAsync(url, data),
                METHOD_DELETE => await client.SendAsync(new HttpRequestMessage()
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = data
                }),
                _ => null,
            };


            /*
            var result = Response.Content.ReadAsStringAsync().Result;
            var node = JsonNode.Parse(
                result,
                new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
            */

        }

        public string GetJsonResponseBody()
        {
            using (var ms = new MemoryStream())
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(this.Response.Content.ReadAsStream(), XmlDictionaryReaderQuotas.Max))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, true, true))
            {
                writer.WriteNode(reader, true);
                writer.Flush();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }



    }
}
