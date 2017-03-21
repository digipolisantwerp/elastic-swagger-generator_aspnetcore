using System;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using ElasticSwaggerGen.Conversion;

namespace ElasticSwaggerGen.Commands
{
    class CommandProvider
    {
        internal static void Create(CommandLineApplication command)
        {
            command.Description = "Create swagger file(s) from the Elastic api-spec files.";
            command.HelpOption("-?|-h|--help");

            var inpathOption = command.Option("-i|--inpath", "The path to the elastic spec file(s) [required].", CommandOptionType.SingleValue);
            var outpathOption = command.Option("-o|--outpath", "The path used for the output file(s) [required].", CommandOptionType.SingleValue);

            command.OnExecute(() => {

                if ( String.IsNullOrWhiteSpace(inpathOption.Value() ) || String.IsNullOrWhiteSpace(outpathOption.Value()) )
                {
                    command.ShowHelp();
                    return 1;
                }

                var inpath = inpathOption.Value();
                var outpath = outpathOption.Value();

                if ( !Directory.Exists(inpath) )
                {
                    command.Error.WriteLine("Input path {0} does not exist.", inpath);
                    return 1;
                }

                if (!Directory.Exists(outpath))
                {
                    command.Error.WriteLine("Output path {0} does not exist.", outpath);
                    return 1;
                }

                command.Out.WriteLine("Converting Elastic JSON spec files from {0} to {1}.", inpath, outpath);

                var converter = new SpecConverter(command, new SpecParser(), new SwaggerWriter());
                return converter.Convert(inpath, outpath);
            });


        }
    }
}
