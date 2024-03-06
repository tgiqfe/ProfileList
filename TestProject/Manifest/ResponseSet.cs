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
    }
}
