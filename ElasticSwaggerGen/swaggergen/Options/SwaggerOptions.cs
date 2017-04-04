using System;
using System.Collections.Generic;

namespace ElasticSwaggerGen.Options
{
    public class SwaggerOptions
    {
        public List<string> ExcludePaths { get; set; } = new List<string>();
        public string BasePath { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactUrl { get; set; }
        public string ApiTitle { get; set; }
        public string ApiDescription { get; set; }
        public string ApiVersion { get; set; }
    }
}
