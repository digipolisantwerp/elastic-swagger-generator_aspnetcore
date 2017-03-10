using System;
using System.Collections.Generic;

namespace ElasticSwaggerGen.Spec
{
    public class EndpointUrl
    {
        public string Path { get; set; }
        public List<string> Paths { get; set; } = new List<string>();
        public List<EndpointUrlPart> Parts { get; set; } = new List<EndpointUrlPart>();
        public List<EndpointParameter> Params { get; set; } = new List<EndpointParameter>();
    }
}
