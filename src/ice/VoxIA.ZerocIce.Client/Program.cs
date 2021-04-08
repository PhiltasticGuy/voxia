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
                var client = new IceMediaClient();
                client.Start(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            Console.ReadLine();

            return 0;
        }
    }
}
