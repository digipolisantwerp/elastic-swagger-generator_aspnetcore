using System;
using System.Collections.Generic;
using ElasticSwaggerGen.Spec;

namespace ElasticSwaggerGen.Conversion
{
    public interface ISwaggerWriter
    {
        void Write(IEnumerable<Endpoint> endpoints, string outPath);
    }
}
