using System;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
using VoxIA.ZerocIce.Core.Server;

namespace VoxIA.ZerocIce.Server
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                var client = new IceClient();
                var song = new Song() { Url = "The_Celebrated_Minuet.mp3" };
                var service = new LibVlcPlaybackService(false, "--no-video");
                await service.InitializeAsync(client, song);

                string choice;
                bool isRunning = true;
                while (isRunning)
                {
                    Console.WriteLine("################################################################################");
                    Console.WriteLine("Choose an action from the following list:");
                    Console.WriteLine("\tplay - Play (Stream) the selected song.");
                    Console.WriteLine("\tpause - Pause the currently playing (streaming) song.");
                    Console.WriteLine("\tstop - Stop the playback (stream).");
                    Console.WriteLine("\tquit - Quit");
                    Console.WriteLine();
                    Console.Write("> What do you want to do? ");
                    choice = Console.ReadLine();
                    Console.WriteLine();

                    switch (choice.Trim().ToLower())
                    {
                        case "play":
                            //await service.PlayAsync(client, song);
                            service.Play();
                            break;

                        case "pause":
                            service.Pause();
                            break;

                        case "stop":
                            service.Stop();
                            break;

                        case "quit":
                        case "exit":
                            isRunning = false;
                            break;

                        default:
                            Console.WriteLine("That action doesn't exist...");
                            break;
                    };

                    Console.WriteLine("################################################################################");
                    Console.WriteLine();
                }

                //var server = new IceMediaServer();

                ////
                //// Destroy the communicator on Ctrl+C or Ctrl+Break
                ////
                //Console.CancelKeyPress += (sender, eventArgs) => server.Stop();

                //server.Start(args, "config.server");

                //service.Stop();
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
