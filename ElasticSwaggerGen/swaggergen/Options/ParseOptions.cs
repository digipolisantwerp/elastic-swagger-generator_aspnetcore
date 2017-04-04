using System;
using System.Collections.Generic;

namespace ElasticSwaggerGen.Options
{
    public class ParseOptions
    {
        public List<string> ExcludeFiles { get; set; } = new List<string>();
        public string CommonParamsFilename { get; set; }

    }
}
