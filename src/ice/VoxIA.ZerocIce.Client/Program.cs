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
                using var client = new ConsoleIceClient();
                client.Start(args);
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
