using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    internal class TestAction
    {
        #region static parameter

        public const string CONTENT_TYPE_FORM = "application/x-www-form-urlencoded";
        public const string CONTENT_TYPE_JSON = "application/json";
        public const string METHOD_GET = "GET";
        public const string METHOD_POST = "POST";
        public const string METHOD_PUT = "PUT";
        public const string METHOD_DELETE = "DELETE";

        #endregion
        #region Serialize parameter

        /// <summary>
        /// APIのアドレス
        /// </summary>
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

        /// <summary>
        /// POST,PUT,DELETEの場合のBodyパラメータ
        /// </summary>
        public Dictionary<string, string> BodpyParameters { get; set; }

        public List<TestResult> TestResults { get; set; }

        #endregion

        [JsonIgnore]
        [YamlIgnore]
        private ResponseSet ResponseSet { get; set; }

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

        /// <summary>
        /// HTTPリクエストを送信
        /// </summary>
        /// <param name="server"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task Send(string server, string address)
        {
            string url = $"{server}{address}";
            this.ResponseSet = new() { Server = server };

            using (var data = new StringContent(
                this.ContentType switch
                {
                    CONTENT_TYPE_FORM => string.Join("&", this.BodpyParameters.Select(x => $"{x.Key}={x.Value}")),
                    CONTENT_TYPE_JSON => JsonSerializer.Serialize(this.BodpyParameters),
                    _ => "",
                }, Encoding.UTF8, this.ContentType))
            using (var client = new HttpClient())
            {
                await this.ResponseSet.SendAsync(client, server, address, this.Method, data);
            }

            if (this.TestResults?.Count > 0)
            {
                foreach (var result in this.TestResults)
                {
                    result.SetResponseParameter(ResponseSet);
                }
            }
        }
    }
}
