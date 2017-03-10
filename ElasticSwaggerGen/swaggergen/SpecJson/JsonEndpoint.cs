using System;
using System.Collections.Generic;
using ElasticSwaggerGen.Spec;
using Newtonsoft.Json.Linq;

namespace ElasticSwaggerGen.SpecJson
{
    public class JsonEndpoint
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Documentation { get; set; }
        public List<string> Methods { get; set; } = new List<string>();
        public JsonEndpointUrl Url { get; set; } = new JsonEndpointUrl();
        public Dictionary<string, JToken> Params { get; set; } = new Dictionary<string, JToken>();
        public EndpointBody Body { get; set; } = new EndpointBody();
    }
}
