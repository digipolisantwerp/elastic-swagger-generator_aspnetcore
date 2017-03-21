using System;
using System.Collections.Generic;
using NSwag;

namespace ElasticSwaggerGen.Swagger
{
    public class Responses
    {
        public static KeyValuePair<string, SwaggerResponse> TechnicalError = new KeyValuePair<string, SwaggerResponse>("500", new SwaggerResponse() { Description = "Technical error." });
        public static KeyValuePair<string, SwaggerResponse> NotFound = new KeyValuePair<string, SwaggerResponse>("404", new SwaggerResponse() { Description = "Not Found." });
        public static KeyValuePair<string, SwaggerResponse> ValidationError = new KeyValuePair<string, SwaggerResponse>("400", new SwaggerResponse() { Description = "Validation error." });
        public static KeyValuePair<string, SwaggerResponse> Created = new KeyValuePair<string, SwaggerResponse>("201", new SwaggerResponse() { Description = "Created." });
        public static KeyValuePair<string, SwaggerResponse> Ok = new KeyValuePair<string, SwaggerResponse>("200", new SwaggerResponse() { Description = "Ok." });
    }
}
