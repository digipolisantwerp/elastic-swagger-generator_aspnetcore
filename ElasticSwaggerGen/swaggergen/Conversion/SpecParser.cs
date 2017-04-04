using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElasticSwaggerGen.Spec;
using ElasticSwaggerGen.SpecJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSwaggerGen.Conversion
{
    public class SpecParser : ISpecParser
    {
        public Endpoint ParseFile(string specFile, bool hasRoot)
        {
            var jsonText = File.ReadAllText(specFile);

            var settings = new JsonSerializerSettings();

            JsonEndpoint ep = null;
            if (hasRoot)
            {
                var epf = JsonConvert.DeserializeObject<JsonEndpointFile>(jsonText);
                var epfToken = epf.Properties.First().Value;
                ep = epfToken.ToObject<JsonEndpoint>();
            }
            else
                ep = JsonConvert.DeserializeObject<JsonEndpoint>(jsonText);

            var endpoint = new Endpoint()
            {
                Description = ep.Description,
                Body = ep.Body,
                Documentation = ep.Documentation,
                Methods = ep.Methods
            };

            PopulateEndpointUrl(endpoint.Url, ep.Url);
            PopulateParams(endpoint.Params, ep.Params);

            return endpoint;
        }

        private void PopulateEndpointUrl(EndpointUrl endpointUrl, JsonEndpointUrl jsonEndpointUrl)
        {
            endpointUrl.Path = jsonEndpointUrl.Path;
            endpointUrl.Paths = jsonEndpointUrl.Paths;

            foreach (var part in jsonEndpointUrl.Parts)
            {
                var endpointUrlPart = part.Value.ToObject<EndpointUrlPart>();
                endpointUrlPart.Name = part.Key;
                endpointUrl.Parts.Add(endpointUrlPart);
            }

            PopulateParams(endpointUrl.Params, jsonEndpointUrl.Params);
        }

        private void PopulateParams(List<EndpointParameter> endpointParams, Dictionary<string, JToken> jsonEndpointParams)
        {
            foreach (var jsonParam in jsonEndpointParams)
            {
                var jsonEndpointParam = jsonParam.Value.ToObject<JsonEndpointParameter>();
                var endpointParam = new EndpointParameter()
                {
                    Name = jsonParam.Key,
                    Description = jsonEndpointParam.Description,
                    Options = jsonEndpointParam.Options,
                    Type = jsonEndpointParam.Type,
                };

                if (jsonEndpointParam.Default.ContainsKey("default"))
                {
                    var defaultValue = jsonEndpointParam.Default["default"];
                    if (defaultValue != null)
                    {
                        if (defaultValue is JArray)
                        {
                            var arrayValue = (JArray)defaultValue;
                            foreach (var child in arrayValue.Children())
                            {
                                endpointParam.Default.Add(child.ToString());
                            }
                        }
                        else
                        {
                            endpointParam.Default.Add(defaultValue.ToString());
                        }
                    }
                }

                endpointParams.Add(endpointParam);
            }
        }
    }
}
