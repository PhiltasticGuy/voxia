using Serilog;
using System;
using VoxIA.ZerocIce.Core.Server;

namespace VoxIA.ZerocIce.Server
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

                var server = new ConsoleIceServer(Log.Logger);

                //
                // Destroy the communicator on Ctrl+C or Ctrl+Break
                //
                Console.CancelKeyPress += (sender, eventArgs) => server.Stop();

                server.Start(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);

                // Allow the user to read the error message.
                Console.ReadLine();

                return 1;
            }

            return 0;
        }
    }
}
