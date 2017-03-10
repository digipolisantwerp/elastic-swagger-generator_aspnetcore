using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ElasticSwaggerGen.SpecJson
{
    public class JsonEndpointUrl
    {
        public string Path { get; set; }
        public List<string> Paths { get; set; } = new List<string>();
        public Dictionary<string, JToken> Parts { get; set; } = new Dictionary<string, JToken>();
        public Dictionary<string, JToken> Params { get; set; } = new Dictionary<string, JToken>();
    }
}
