﻿using System.Text.Json.Nodes;

namespace ProfileList.Lib.Api
{
    public class LogParameter
    {
        public int? Line { get; set; }
        public bool? AllPrint { get; set; }

        public static async Task<LogParameter> SetParamAsync(HttpContext context)
        {
            LogParameter parameter = null;
            string body = "";
            using (var reader = new StreamReader(context.Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            switch (context.Request.ContentType)
            {
                case "application/json":
                    Item.Logger.WriteLine("Content-Type: application/json");
                    var node = JsonNode.Parse(body);
                    parameter = new();
                    parameter.Line = int.TryParse(node["line"]?.ToString(), out int line_1) ? line_1 : null;
                    parameter.AllPrint = bool.TryParse(node["allprint"]?.ToString(), out bool allprint_1) ? allprint_1 : null;
                    break;
                case "application/x-www-form-urlencoded":
                    Item.Logger.WriteLine("Content-Type: application/x-www-form-urlencoded");
                    parameter = new();
                    foreach (var leaf in body.Split("&"))
                    {
                        var key = leaf.Substring(0, leaf.IndexOf("="));
                        var val = leaf.Substring(leaf.IndexOf("=") + 1);
                        switch (key)
                        {
                            case "line":
                                parameter.Line = int.TryParse(val, out int line_2) ? line_2 : null;
                                break;
                            case "allprint":
                            case "all":
                                parameter.AllPrint = bool.TryParse(val, out bool allprint_2) ? allprint_2 : null;
                                break;
                        }
                    }
                    break;
                default:
                    Item.Logger.WriteLine($"Content-Type is no supported type. [{context.Request.ContentType}]");
                    break;
            }

            return parameter;
        }

    }
}