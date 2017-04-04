using System;
using System.Collections.Generic;

namespace ElasticSwaggerGen.Spec
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Documentation { get; set; }
        public List<string> Methods { get; set; } = new List<string>();
        public EndpointUrl Url { get; set; } = new EndpointUrl();
        public List<EndpointParameter> Params { get; set; } = new List<EndpointParameter>();
        public EndpointBody Body { get; set; } = new EndpointBody();
        public string FileName { get; set; }

        public void AddCommonParams(IEnumerable<EndpointParameter> commonParams)
        {
            Url.Params.AddRange(commonParams);
        }

        public override string ToString()
        {
            var msg = $"Endpoint url : {Url?.Path ?? "null"}";
            return msg;
        }
    }
}
