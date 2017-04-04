using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElasticSwaggerGen.Options;
using ElasticSwaggerGen.Spec;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElasticSwaggerGen.Conversion
{
    public class SpecConverter : ISpecConverter
    {
        public SpecConverter(ILogger<SpecConverter> logger, ISpecParser parser, ISwaggerWriter writer, IOptions<ParseOptions> options)
        {
            _logger = logger;
            _parser = parser;
            _writer = writer;
            _options = options.Value;
        }

        private readonly ILogger<SpecConverter> _logger;
        private readonly ISpecParser _parser;
        private readonly ISwaggerWriter _writer;
        private readonly ParseOptions _options;

        public int Convert(string inPath, string outPath)
        {
            if (!Directory.Exists(inPath)) throw new DirectoryNotFoundException($"Input path {inPath} does not exist.");

            var inputFiles = GetInputFiles(inPath);

            _logger.LogInformation("Converting Elastic JSON spec files from {inPath} to {outPath}.", inPath, outPath);

            var commonParams = GenerateCommonParams(inPath);
            var endpoints = GenerateEndpoints(inputFiles, commonParams);

            _writer.Write(endpoints, outPath);

            return 0;
        }

        private IEnumerable<string> GetInputFiles(string inPath)
        {
            var inputFiles = Directory.EnumerateFiles(inPath, "*.json");

            //var dir = @"D:\github\elastic\elasticsearch\rest-api-spec\src\main\resources\rest-api-spec\api";
            //var inputFiles = new List<string>() { Path.Combine(dir, "update_by_query.json") };

            if (inputFiles.Count() == 0) throw new JsonFilesNotFoundException(inPath);

            return inputFiles;
        }

        private IEnumerable<EndpointParameter> GenerateCommonParams(string inPath)
        {
            _logger.LogInformation("Reading _common.json...");

            var commonFile = Path.Combine(inPath, "_common.json");
            var endpoint = _parser.ParseFile(commonFile, false);

            _logger.LogInformation("Common parameters generated from file _common.json");

            return endpoint.Params;
        }

        private IEnumerable<Endpoint> GenerateEndpoints(IEnumerable<string> inputFiles, IEnumerable<EndpointParameter> commonParams)
        {
            var endpoints = new List<Endpoint>();

            foreach (var specFile in inputFiles)
            {
                if (_options.ExcludeFiles.Contains(Path.GetFileName(specFile)))
                {
                    _logger.LogInformation("Skipping {specFile}...", specFile);
                    continue;
                }

                _logger.LogInformation("Reading {specFile}...", specFile);
                var endpoint = _parser.ParseFile(specFile, true);

                endpoint.AddCommonParams(commonParams);

                endpoints.Add(endpoint);
                _logger.LogInformation(endpoint.ToString());
            }

            return endpoints;
        }
    }
}
