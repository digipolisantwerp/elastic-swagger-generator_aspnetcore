using System;

namespace ElasticSwaggerGen.Conversion
{
    public interface ISpecConverter
    {
        int Convert(string inPath, string outPath);
    }
}
