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
                var server = new IceMediaServer();

                //
                // Destroy the communicator on Ctrl+C or Ctrl+Break
                //
                Console.CancelKeyPress += (sender, eventArgs) => server.Stop();

                server.Start(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            return 0;
        }
    }
}
