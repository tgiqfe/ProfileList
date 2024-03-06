using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace TestProject.Manifest
{
    public class TestAction
    {
        private string _server_URL = null;
        private int _server_Port = 0;

        public string Address { get; set; }

        private string _method = "GET";
        public string Method
        {
            get { return _method; }
            set
            {
                _method = value.ToLower() switch
                {
                    "get" => "GET",
                    "post" => "POST",
                    "put" => "PUT",
                    "delete" => "DELETE",
                    _ => "GET"
                };
            }
        }

        private readonly static string[] _validContentTypes = new string[] { "application/json", "application/x-www-form-urlencoded" };
        private string _contentType = null;
        public string ContentType
        {
            get
            {
                if (_contentType == null && this.BodpyParameters?.Count > 0)
                {
                    _contentType = "application/x-www-form-urlencoded";
                }
                return _contentType;
            }
            set
            {
                _contentType = _validContentTypes.Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase)) ? value : "";
            }
        }

        public Dictionary<string, string> BodpyParameters { get; set; }

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
    }
}
