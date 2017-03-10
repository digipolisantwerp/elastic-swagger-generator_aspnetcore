using System;

namespace ElasticSwaggerGen.Spec
{
    public class EndpointFile
    {
        public string Name { get; set; }
        public Endpoint EndPoint { get; set; } = new Endpoint();
    }
}
