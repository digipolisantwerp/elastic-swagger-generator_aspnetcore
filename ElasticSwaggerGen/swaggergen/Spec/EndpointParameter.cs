using System;
using System.Collections.Generic;

namespace ElasticSwaggerGen.Spec
{
    public class EndpointParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Default { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }
}
