using System;
using ElasticSwaggerGen.Conversion;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSwaggerGen
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddSingleton<ISpecConverter, SpecConverter>();
            services.AddSingleton<ISpecParser, SpecParser>();
            services.AddSingleton<ISwaggerWriter, SwaggerWriter>();

            return services;
        }
    }
}
