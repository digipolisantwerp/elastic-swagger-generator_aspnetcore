using System;
using ElasticSwaggerGen.Spec;

namespace ElasticSwaggerGen.Conversion
{
    public interface ISpecParser
    {
        Endpoint ParseFile(string specFile, bool hasRoot);
    }
}
