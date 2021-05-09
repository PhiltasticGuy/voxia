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
                var server = new ConsoleIceServer();

                //
                // Destroy the communicator on Ctrl+C or Ctrl+Break
                //
                Console.CancelKeyPress += (sender, eventArgs) => server.Stop();

                server.Start(args, "config.server");
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
