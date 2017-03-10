using System;

namespace ElasticSwaggerGen.Spec
{
    public class EndpointBody
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string Serialize { get; set; }

    }
}
