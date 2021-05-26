using Serilog;
using System;
using VoxIA.ZerocIce.Core.Client;

namespace VoxIA.ZerocIce.Client
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                // Create the Serilog logger instance.
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    //.WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                using var client = new ConsoleIceClient(Log.Logger);
                client.Start(args);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Exception has occured!");

                // Allow the user to read the error message.
                Console.ReadLine();

                return 1;
            }

            return 0;
        }
    }
}
