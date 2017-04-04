using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElasticSwaggerGen.Options;
using ElasticSwaggerGen.Spec;
using ElasticSwaggerGen.Swagger;
using Microsoft.Extensions.Options;
using NSwag;

namespace ElasticSwaggerGen.Conversion
{
    public class SwaggerWriter : ISwaggerWriter
    {
        public SwaggerWriter(IOptions<SwaggerOptions> options)
        {
            _options = options.Value;
        }

        private readonly SwaggerOptions _options;

        public void Write(IEnumerable<Endpoint> endpoints, string outPath)
        {
            var document = new SwaggerDocument();

            document.Info = GenerateInfo();
            document.BasePath = _options.BasePath;

            foreach (var endpoint in endpoints)
            {
                var paths = endpoint.Url.Paths.Count > 0 ? endpoint.Url.Paths : new List<string>() { endpoint.Url.Path };
                foreach ( var path in paths )
                {
                    if ( _options.ExcludePaths.Contains(path) ) continue;
                    var pathAlreadyExists = document.Paths.Keys.Contains(path);
                    var operations = pathAlreadyExists ? document.Paths[path] : new SwaggerOperations();
                    PopulateOperationsByPath(operations, path, endpoint);
                    if ( !pathAlreadyExists ) document.Paths.Add(path, operations);
                }
            }

            document.GenerateOperationIds();

            var json = document.ToJson();

            // TODO (SVB): create outpath if not exists
            //if (!Directory.Exists(outPath)) throw new DirectoryNotFoundException($"Output path {outPath} does not exist.");

            File.WriteAllText(Path.Combine(outPath, $"elastic-swagger-{DateTime.Now.ToString("yyyyMMddHHmmss")}.json"), json);
        }

        private SwaggerInfo GenerateInfo()
        {
            var info = new SwaggerInfo()
            {
                Contact = new SwaggerContact()
                {
                    Email = _options.ContactEmail,
                    Name = _options.ContactName,
                    Url = _options.ContactUrl
                },
                Description = _options.ApiDescription,
                License = new SwaggerLicense()
                {
                    Name = "?",
                    Url = "?"
                },
                TermsOfService = "?",
                Title = _options.ApiTitle,
                Version = _options.ApiVersion
            };

            return info;
        }

        private SwaggerOperations PopulateOperationsByPath(SwaggerOperations operations, string path, Endpoint endpoint)
        {
            foreach ( var method in endpoint.Methods )
            {
                var operation = new SwaggerOperation()
                {
                    Description = $"Source: {endpoint.FileName}",
                    Produces = new List<string>() { "application/json" },
                    // TODO (SVB): nog aanvullen ?                        
                };

                //foreach ( var endpointParam in endpoint.Params )
                //{
                //    var paramType = ParseJsonObjectType(endpointParam.Type);
                //    var defaultValue = ParseDefault(endpointParam.Default, paramType);
                //    var param = new SwaggerParameter()
                //    {
                //        Description = endpointParam.Description,
                //        Default = defaultValue,
                //        Type = paramType,
                //        Name = endpointParam.Name,
                //        Kind = SwaggerParameterKind.Path,
                //        IsRequired = endpointParam.Required
                //        // TODO (SVB): nog aanvullen
                //    };

                //    if ( param.Type != NJsonSchema.JsonObjectType.Null ) operation.Parameters.Add(param);
                //}

                foreach ( var urlParam in endpoint.Url.Params )
                {
                    var paramType = ParseJsonObjectType(urlParam.Type);
                    var defaultValue = ParseDefault(urlParam.Default, paramType);
                    var param = new SwaggerParameter()
                    {
                        Description = urlParam.Description,
                        Default = defaultValue,
                        Type = paramType,
                        Name = urlParam.Name,
                        Kind = SwaggerParameterKind.Query,
                        IsRequired = urlParam.Required
                        // TODO (SVB): nog aanvullen ?
                    };

                    if ( param.Type == NJsonSchema.JsonObjectType.Array )
                    {
                        param.Item = new NJsonSchema.JsonSchema4() { Type = NJsonSchema.JsonObjectType.String };
                    }

                    if ( param.Type != NJsonSchema.JsonObjectType.Null ) operation.Parameters.Add(param);
                }

                foreach (var urlPart in endpoint.Url.Parts)
                {
                    var partType = ParseJsonObjectType(urlPart.Type);
                    if (partType == NJsonSchema.JsonObjectType.Array) partType = NJsonSchema.JsonObjectType.String;

                    var param = new SwaggerParameter()
                    {
                        Description = urlPart.Description,
                        Type = partType,
                        Name = urlPart.Name,
                        Kind = SwaggerParameterKind.Path,
                        IsRequired = true
                        // TODO (SVB): nog aanvullen ?
                    };

                    if ( param.Type == NJsonSchema.JsonObjectType.Array )
                    {
                        param.Item = new NJsonSchema.JsonSchema4() { Type = NJsonSchema.JsonObjectType.String };
                    }

                    if ( param.Type != NJsonSchema.JsonObjectType.Null && path.Contains($"{{{param.Name}}}") ) operation.Parameters.Add(param);
                }

                var operationMethod = (SwaggerOperationMethod)Enum.Parse(typeof(SwaggerOperationMethod), method, true);
                AddResponses(operation, operationMethod);

                operations.Add(operationMethod, operation);
            }

            return operations;
        }

        private NJsonSchema.JsonObjectType ParseJsonObjectType(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return NJsonSchema.JsonObjectType.Null;
            if (type == "null")
                return NJsonSchema.JsonObjectType.Null;
            if (type == "enum")
                return NJsonSchema.JsonObjectType.String;
            if (type == "time")
                return NJsonSchema.JsonObjectType.String;
            if (type == "list")
                return NJsonSchema.JsonObjectType.Array;
            else
                return (NJsonSchema.JsonObjectType)Enum.Parse(typeof(NJsonSchema.JsonObjectType), type, true);
        }

        private object ParseDefault(List<string> defaults, NJsonSchema.JsonObjectType objectType)
        {
            var value = defaults.FirstOrDefault();
            if ( value == null ) return null;

            switch (objectType)
            {
                case NJsonSchema.JsonObjectType.Array:
                    // ?
                    break;
                case NJsonSchema.JsonObjectType.Boolean:
                    var defaultBoolValue = false;
                    if (!Boolean.TryParse(value, out defaultBoolValue))
                        return false;
                    else
                        return defaultBoolValue;
                case NJsonSchema.JsonObjectType.Integer:
                    var defaultIntValue = 0;
                    if (Int32.TryParse(value.ToString(), out defaultIntValue))
                        return null;
                    else
                        return defaultIntValue;
                case NJsonSchema.JsonObjectType.Null:
                    return null;
                case NJsonSchema.JsonObjectType.Number:
                    var doubleDefaultValue = 0.00;
                    if (!Double.TryParse(value, out doubleDefaultValue))
                        return null;
                    else
                        return doubleDefaultValue;
                case NJsonSchema.JsonObjectType.File:
                    // ?                    
                    break;
            }

            return value;
        }

        private void AddResponses(SwaggerOperation operation, SwaggerOperationMethod operationMethod)
        {
            operation.Responses.Add("500", new SwaggerResponse() { Description = "Technical error." });

            switch (operationMethod)
            {
                case SwaggerOperationMethod.Undefined:

                    break;
                case SwaggerOperationMethod.Get:
                    operation.Responses.Add(Responses.Ok);
                    operation.Responses.Add(Responses.NotFound);
                    break;
                case SwaggerOperationMethod.Post:
                    operation.Responses.Add(Responses.Created);     // TODO (SVB): nog te checken wat deze returnen
                    operation.Responses.Add(Responses.ValidationError);
                    break;
                case SwaggerOperationMethod.Put:
                    operation.Responses.Add(Responses.Ok);     // TODO (SVB): nog te checken wat deze returnen
                    operation.Responses.Add(Responses.ValidationError);
                    operation.Responses.Add(Responses.NotFound);
                    break;
                case SwaggerOperationMethod.Delete:
                    operation.Responses.Add(Responses.Ok);     // TODO (SVB): nog te checken wat deze returnen
                    operation.Responses.Add(Responses.NotFound);
                    break;
                case SwaggerOperationMethod.Options:
                    operation.Responses.Add(Responses.Ok);
                    break;
                case SwaggerOperationMethod.Head:
                    operation.Responses.Add(Responses.Ok);
                    break;
                case SwaggerOperationMethod.Patch:
                    operation.Responses.Add(Responses.Ok);
                    break;
            }
        }
    }
}
