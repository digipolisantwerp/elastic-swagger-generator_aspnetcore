using System;
using System.Collections.Generic;
using System.IO;
using ElasticSwaggerGen.Spec;
using ElasticSwaggerGen.SpecJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using ElasticSwaggerGen.Logging;

namespace ElasticSwaggerGen.Conversion
{
    class SpecConverter
    {
        public SpecConverter(CommandLineApplication command, SpecParser parser, SwaggerWriter writer)
        {
            _command = command;
            _parser = parser;
            _writer = writer;
        }

        private readonly CommandLineApplication _command;
        private readonly SpecParser _parser;
        private readonly SwaggerWriter _writer;

        private readonly List<string> _excludeFiles = new List<string>();

        private void InitExcludes()
        {
            _excludeFiles.Add("cluster.put_settings.json");
        }

        public int Convert(string inPath, string outPath)
        {
            InitExcludes();

            var inputFiles = Directory.EnumerateFiles(inPath, "*.json").Where(f => !f.EndsWith("_common.json"));
            if ( inputFiles.Count() == 0 )
            {
                _command.Error.WriteLine("No Elastic JSON spec files  in {0}.", inPath);
                return 1;
            }

            var endpoints = new List<Endpoint>();

            foreach ( var specFile in inputFiles )
            {
                if ( _excludeFiles.Contains(Path.GetFileName(specFile)))
                {
                    _command.Out.WriteLine("Skipping {0}...", specFile);
                    continue;
                }

                _command.Out.WriteLine("Converting {0}...", specFile);
                var endpoint = ParseFile(specFile);
                endpoints.Add(endpoint);
                _command.Out.WriteLine(endpoint.ToString());
            }

            _writer.Write(endpoints, outPath);

            return 0;
        }

        private Endpoint ParseFile(string specFile)
        {
            var jsonText = File.ReadAllText(specFile);

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

            foreach (var jsonParam in jsonEndpointUrl.Params)
            {
                var jsonEndpointParam = jsonParam.Value.ToObject<JsonEndpointParameter>();
                var endpointParam = new EndpointParameter()
                {
                     Name = jsonParam.Key,
                     Description = jsonEndpointParam.Description,
                     Options = jsonEndpointParam.Options,
                     Type = jsonEndpointParam.Type,
                };

                if ( jsonEndpointParam.Default.ContainsKey("default") )
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

                endpointUrl.Params.Add(endpointParam);
            }
        }

        private void PopulateParams(List<EndpointParameter> endpointParams, Dictionary<string, JToken> jsonEndpointParams)
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
