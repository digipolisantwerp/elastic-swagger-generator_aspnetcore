using System;
using Microsoft.Extensions.CommandLineUtils;
using ElasticSwaggerGen.Commands;

namespace ElasticSwaggerGen
{
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "swaggergen";
            app.HelpOption("-?|-h|--help");

            app.Command("create", CommandProvider.Create);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            app.Execute(args);
            
            return;
        }
    }
}

// TODO (SVB): what about _common.json