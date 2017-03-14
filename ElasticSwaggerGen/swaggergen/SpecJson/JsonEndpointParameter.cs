using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ElasticSwaggerGen.SpecJson
{
    public class JsonEndpointParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JToken> Default { get; set; } = new Dictionary<string, JToken>();
        public List<string> Options { get; set; } = new List<string>();
    }
}
