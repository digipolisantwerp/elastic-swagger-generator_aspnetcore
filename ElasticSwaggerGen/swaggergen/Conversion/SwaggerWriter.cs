using System;
using System.Collections.Generic;
using ElasticSwaggerGen.Spec;
using NSwag;
using System.Linq;
using ElasticSwaggerGen.Swagger;

namespace ElasticSwaggerGen.Conversion
{
    public class SwaggerWriter
    {
        public void Write(IEnumerable<Endpoint> endpoints, string outPath)
        {
            var document = new SwaggerDocument();

            document.Info = GenerateInfo();
            document.BasePath = "/logging-app1";

            foreach ( var endpoint in endpoints )
            {
                var pathAlreadyExists = document.Paths.Keys.Contains(endpoint.Url.Path);
                var operations = pathAlreadyExists ? document.Paths[endpoint.Url.Path] : new SwaggerOperations();
                PopulateOperations(operations, endpoint);
                if ( !pathAlreadyExists ) document.Paths.Add(endpoint.Url.Path, operations);
            }

            document.GenerateOperationIds();
            
            var json = document.ToJson();
            var s = 1;
        }

        private SwaggerInfo GenerateInfo()
        {
            var info = new SwaggerInfo()
            {
                Contact = new SwaggerContact()
                {
                    Email = "da_apie_team@digipolis.be",
                    Name = "ACPaaS",
                    Url = "?"
                },
                Description = "ElasticSearch API",
                License = new SwaggerLicense()
                {
                    Name = "?",
                    Url = "?"
                },
                TermsOfService = "?",
                Title = "ElasticSearch",
                Version = "5"
            };

            return info;
        }

        private SwaggerOperations PopulateOperations(SwaggerOperations operations, Endpoint endpoint)
        {
            foreach (var method in endpoint.Methods)
            {
                var operation = new SwaggerOperation()
                {
                    Description = endpoint.Description,
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

                foreach ( var urlPart in endpoint.Url.Parts )
                {
                    var partType = ParseJsonObjectType(urlPart.Type);
                    if ( partType == NJsonSchema.JsonObjectType.Array ) partType = NJsonSchema.JsonObjectType.String;

                    var param = new SwaggerParameter()
                    {
                        Description = urlPart.Description,
                        Type = partType,
                        Name = urlPart.Name,
                        Kind = SwaggerParameterKind.Path,
                        IsRequired = urlPart.Required
                        // TODO (SVB): nog aanvullen ?
                    };

                    if ( param.Type == NJsonSchema.JsonObjectType.Array )
                    {
                        param.Item = new NJsonSchema.JsonSchema4() { Type = NJsonSchema.JsonObjectType.String };
                    }

                    if ( param.Type != NJsonSchema.JsonObjectType.Null ) operation.Parameters.Add(param);
                }

                var operationMethod = (SwaggerOperationMethod)Enum.Parse(typeof(SwaggerOperationMethod), method, true);
                AddResponses(operation, operationMethod);

                operations.Add(operationMethod, operation);
            }

            return operations;
        }

        private NJsonSchema.JsonObjectType ParseJsonObjectType(string type)
        {
            if ( String.IsNullOrWhiteSpace(type) )
                return NJsonSchema.JsonObjectType.Null;
            if ( type == "null" )
                return NJsonSchema.JsonObjectType.Null;
            if ( type == "enum" )
                return NJsonSchema.JsonObjectType.String;
            if ( type == "time" )
                return NJsonSchema.JsonObjectType.String;
            if ( type == "list" )
                return NJsonSchema.JsonObjectType.Array;
            else
                return (NJsonSchema.JsonObjectType)Enum.Parse(typeof(NJsonSchema.JsonObjectType), type, true);
        }

        private object ParseDefault(List<string> defaults, NJsonSchema.JsonObjectType objectType)
        {
            var value = defaults.FirstOrDefault();
            if ( value == null ) return null;

            switch ( objectType )
            {
                case NJsonSchema.JsonObjectType.Array:
                    // ?
                    break;
                case NJsonSchema.JsonObjectType.Boolean:
                    var defaultBoolValue = false;
                    if ( !Boolean.TryParse(value, out defaultBoolValue) )
                        return false;
                    else
                        return defaultBoolValue;
                case NJsonSchema.JsonObjectType.Integer:
                    var defaultIntValue = 0;
                    if ( Int32.TryParse(value.ToString(), out defaultIntValue) )
                        return null;
                    else
                        return defaultIntValue;
                case NJsonSchema.JsonObjectType.Null:
                    return null;
                case NJsonSchema.JsonObjectType.Number:
                    var doubleDefaultValue = 0.00;
                    if ( !Double.TryParse(value, out doubleDefaultValue) )
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
