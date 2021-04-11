using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Client
{
    public class IceMediaClient : IMediaClient, IDisposable
    {
        private readonly LibVLC _vlc;
        private readonly MediaPlayer _player;
        private readonly Media _media;

        private bool disposedValue;

        public IceMediaClient() : this(false, "--no-video")
        {
        }

        public IceMediaClient(bool enableDebugLogs, params string[] options)
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC(enableDebugLogs, options);

            if (!enableDebugLogs)
            {
                _vlc.Log += (sender, e) => { /* Do nothing! Don't log in the console... */ };
            }

            _player = new MediaPlayer(_vlc);
            _media = new Media(_vlc, new Uri("http://localhost:5000/stream.mp3"));
        }

        public void Start(string[] args)
        {
            using var communicator = Ice.Util.initialize(ref args);
            RunIceClient(communicator);
        }

        public void Stop()
        {
        }

        private void RunIceClient(Ice.Communicator communicator)
        {
            try
            {
                var obj = communicator.stringToProxy("SimplePrinter:default -h localhost -p 10000");
                var mediaServer = MediaServerPrxHelper.checkedCast(obj);

                if (mediaServer == null)
                {
                    throw new ApplicationException("Invalid proxy");
                }

                string choice;
                bool isRunning = true;
                while (isRunning)
                {
                    Console.WriteLine("################################################################################");
                    Console.WriteLine("Choose an action from the following list:");
                    Console.WriteLine("\tlist  - List available songs in the library.");
                    Console.WriteLine("\tfind  - Find songs by Title or by Artist.");
                    Console.WriteLine("\tplay  - Play (Stream) the selected song.");
                    Console.WriteLine("\tpause - Pause the currently playing (streaming) song.");
                    Console.WriteLine("\tstop  - Stop the playback (stream).");
                    Console.WriteLine("\tquit  - Quit");
                    Console.WriteLine();
                    Console.Write("> What do you want to do? ");
                    choice = Console.ReadLine();
                    Console.WriteLine();

                    switch (choice.Trim().ToLower())
                    {
                        case "list":
                            DisplayAvailableSongs(mediaServer);
                            break;

                        case "find":
                            FindSongs(mediaServer);
                            break;

                        case "play":
                            DisplayAvailableSongs(mediaServer);

                            Console.WriteLine();
                            Console.Write("> Enter filename of the song that you want to play: ");
                            choice = Console.ReadLine();
                            Console.WriteLine();

                            if (mediaServer.PlaySong("1", choice))
                            {
                                _player.Play(_media);
                            }
                            else
                            {
                                Console.WriteLine($"Could not play the song '{choice}'. Please try again.");
                            }
                            break;

                        case "pause":
                            mediaServer.PauseSong("1");
                            break;

                        case "stop":
                            mediaServer.StopSong("1");
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

                //var obj = communicator.stringToProxy("SimplePrinter:default -h localhost -p 10000");
                //var printer = MediaServerPrxHelper.checkedCast(obj);
                //if (printer == null)
                //{
                //    throw new ApplicationException("Invalid proxy");
                //}

                //var content = File.ReadAllBytes($"./local-lib/lorem.txt");
                //var results = printer.begin_uploadFile(content);
                ////printer.uploadFileAsync(content);

                //Task.Run(() =>
                //{
                //    string filename = "lorem.txt";
                //    string filepath = $"./local-lib/{filename}";
                //    const int chunkSize = 1140;
                //    int offset = 0;
                //    using var fs = File.OpenRead(filepath);
                //    using var br = new BinaryReader(fs);

                //    while (br.PeekChar() != -1)
                //    {
                //        byte[] chunk = br.ReadBytes(chunkSize);
                //        printer.uploadFileChunkAsync(filename, offset, chunk);
                //        offset += chunk.Length;

                //        string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                //        Console.WriteLine(utfString);
                //    }
                //});

                //Task.Run(() =>
                //{
                //    string filename = "lorem2.txt";
                //    string filepath = $"./local-lib/{filename}";
                //    const int chunkSize = 1026;
                //    int offset = 0;
                //    using var fs = File.OpenRead(filepath);
                //    using var br = new BinaryReader(fs);

                //    while (br.PeekChar() != -1)
                //    {
                //        byte[] chunk = br.ReadBytes(chunkSize);
                //        printer.uploadFileChunkAsync(filename, offset, chunk);
                //        offset += chunk.Length;

                //        string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                //        Console.WriteLine(utfString);
                //    }
                //});

                //Console.WriteLine("Test #1");
                //Console.WriteLine("Test #2");
                //Console.WriteLine("Test #3");
                //printer.printString("Hello World!");
                ////Console.WriteLine(printer.getLibraryContent());

                //printer.end_uploadFile(results);
            }
            finally
            {
                communicator.destroy();
            }
        }

        private void FindSongs(MediaServerPrx mediaServer)
        {
            Console.Write("> Enter search query for song title or artist name: ");
            var choice = Console.ReadLine();
            Console.WriteLine();

            var songs = mediaServer.FindSongs(choice);
            foreach (Song song in songs)
            {
                Console.WriteLine($"  * {song.Title} - {song.Artist} ({song.Url})");
            }
        }

        private static void DisplayAvailableSongs(MediaServerPrx mediaServer)
        {
            var songs = mediaServer.GetAllSongs();
            foreach (Song song in songs)
            {
                Console.WriteLine($"  * {song.Title} - {song.Artist} ({song.Url})");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_media != null) _media.Dispose();
                    if (_player != null) _player.Dispose();
                    if (_vlc != null) _vlc.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~IceMediaClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
