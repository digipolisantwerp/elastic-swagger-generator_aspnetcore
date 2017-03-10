using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElasticSwaggerGen.Spec;
using ElasticSwaggerGen.SpecJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSwaggerGen
{
    public class Program
    {
        static string _filename = @"D:\github\elastic\elasticsearch\rest-api-spec\src\main\resources\rest-api-spec\api\bulk.json";

        static void Main(string[] args)
        {
            var jsonText = File.ReadAllText(_filename);

            var settings = new JsonSerializerSettings();

            var epf = JsonConvert.DeserializeObject<JsonEndpointFile>(jsonText);
            var epfToken = epf.Properties.First().Value;
            var ep = epfToken.ToObject<JsonEndpoint>();

            var endpoint = new Endpoint()
            {
                Name = epfToken.Path,
                Description = ep.Description,
                Body = ep.Body,
                Documentation = ep.Documentation,
                Methods = ep.Methods
            };

            PopulateEndpointUrl(endpoint.Url, ep.Url);
            PopulateParams(endpoint.Params, ep.Params);

            Console.ReadKey();
        }

        static void PopulateEndpointUrl(EndpointUrl endpointUrl, JsonEndpointUrl jsonEndpointUrl)
        {
            endpointUrl.Path = jsonEndpointUrl.Path;
            endpointUrl.Paths = jsonEndpointUrl.Paths;

            foreach (var part in jsonEndpointUrl.Parts)
            {
                var endpointUrlPart = part.Value.ToObject<EndpointUrlPart>();
                endpointUrlPart.Name = part.Key;
                endpointUrl.Parts.Add(endpointUrlPart);
            }

            foreach (var param in jsonEndpointUrl.Params)
            {
                var endpointParameter = param.Value.ToObject<EndpointParameter>();
                endpointParameter.Name = param.Key;
                endpointUrl.Params.Add(endpointParameter);
            }
        }

        static void PopulateParams(List<EndpointParameter> endpointParams, Dictionary<string, JToken> jsonEndpointParams)
        {
            foreach (var jsonParam in jsonEndpointParams)
            {
                var parameter = jsonParam.Value.ToObject<EndpointParameter>();
                parameter.Name = jsonParam.Key;
                endpointParams.Add(parameter);
            }
        }
    }
}