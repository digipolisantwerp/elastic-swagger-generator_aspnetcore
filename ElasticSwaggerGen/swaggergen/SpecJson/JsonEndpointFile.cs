using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSwaggerGen.SpecJson
{
    public class JsonEndpointFile
    {
        public string Name { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JToken> Properties { get; set; } = new Dictionary<string, JToken>();
    }
}
