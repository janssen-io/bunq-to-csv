using Bunq.Sdk.Context;
using BunqDownloader.Bunq;
using BunqDownloader.Converter;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace BunqDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--help") || args.Contains("-h"))
            {
                CommandlineArguments.PrintHelp();
                return;
            }

            IConfiguration config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            CommandlineArguments cmdArgs = new CommandlineArguments();
            config.Bind(cmdArgs);

            try
            {
                Execute(cmdArgs);
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (cmdArgs.Debug)
                {
                    throw;
                }

                Console.Error.WriteLine(e.Message);
                Console.ResetColor();
            }
        }

        static void Execute(CommandlineArguments parameters)
        {
            SetupContext(parameters);

            var pagerConfigPath = Path.Combine(parameters.ConfigDirectory, "pager.config");
            var pagerConfigLoader = new PagerConfigurationLoader(pagerConfigPath);
            var pagerConfig = pagerConfigLoader.Read();
            var converter = new PaymentConverter(pagerConfig, parameters);
            converter.Run();

            pagerConfigLoader.Write(pagerConfig.WithLastUpToDate(parameters.UpToDate));
        }

        private static void SetupContext(CommandlineArguments parameters)
        {
            IApiKeyReader apiKeyReader = new CommandlineApiKeyReader();
            var apiEnv = ApiEnvironmentType.PRODUCTION;
            string config = Path.Combine(parameters.ConfigDirectory, "production.config");

            if (parameters.Sandbox)
            {
                apiKeyReader = new SandboxApiKeyReader();
                apiEnv = ApiEnvironmentType.SANDBOX;
                config = Path.Combine(parameters.ConfigDirectory, "sandbox.config");
            }

            new Initializer(apiEnv, apiKeyReader, config).InitializeApiContext();
        }
    }
}
