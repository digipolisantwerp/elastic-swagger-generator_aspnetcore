using System;
using System.IO;
using ElasticSwaggerGen.Conversion;
using Microsoft.Extensions.CommandLineUtils;

namespace ElasticSwaggerGen.Commands
{
    public class SwaggerGenApplication : CommandLineApplication
    {
        public SwaggerGenApplication(ISpecConverter converter) : base(true)
        {
            _converter = converter;
            Initialize();
        }

        private readonly ISpecConverter _converter;

        private void Initialize()
        {
            this.Name = "swaggergen";
            this.HelpOption("-?|-h|--help");

            this.Command("create", Create);

            this.OnExecute(() =>
            {
                this.ShowHelp();
                return 0;
            });
        }

        private void Create(CommandLineApplication command)
        {
            command.Description = "Create swagger file(s) from the Elastic api-spec files.";
            command.HelpOption("-?|-h|--help");

            var inpathOption = command.Option("-i|--inpath", "The path to the elastic spec file(s) [required].", CommandOptionType.SingleValue);
            var outpathOption = command.Option("-o|--outpath", "The path used for the output file(s) [required].", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var inpath = inpathOption.Value();
                var outpath = outpathOption.Value();

                if ( String.IsNullOrWhiteSpace(inpath) || String.IsNullOrWhiteSpace(outpath) )
                {
                    command.ShowHelp();
                    return 1;
                }

                try
                {
                    return _converter.Convert(inpath, outpath);
                }
                catch ( Exception ex)
                {
                    this.Error.WriteLine(ex.Message);
                    return 1;
                }
            });
        }
    }
}
