using LibVLCSharp.Shared;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Client
{
    public class ConsoleIceClient : IIceClient, IDisposable
    {
        private readonly ILogger _logger;
        private readonly Guid _clientId = Guid.NewGuid();
        private readonly LibVLC _vlc;
        private readonly MediaPlayer _player;
        private Media _media;

        private bool _disposedValue = false;
        private bool _secure = false;

        public ConsoleIceClient(ILogger logger) : this(logger, false, "--no-video")
        {
        }

        public ConsoleIceClient(ILogger logger, bool enableDebugLogs, params string[] options)
        {
            _logger = logger;

            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC(enableDebugLogs, options);

            if (!enableDebugLogs)
            {
                _vlc.Log += (sender, e) => { /* Do nothing! Don't log in the console... */ };
            }

            _player = new MediaPlayer(_vlc);
            //TODO: Port must be assigned from server after registering!
            //_media = new Media(_vlc, new Uri("http://localhost:6000/stream.mp3"));
        }

        public void Start(string[] args)
        {
            using var communicator = Ice.Util.initialize(ref args, "config.client");
            RunIceClient(communicator);
        }

        public void Stop()
        {
        }

        private void RunIceClient(Ice.Communicator communicator)
        {
            try
            {
                //var obj = communicator.propertyToProxy("MediaServer.Proxy");
                //var obj = communicator.stringToProxy("MediaServerId@SimpleServer.MediaServerAdapter");
                var obj = communicator.stringToProxy("MediaServer");
                obj.ice_connectionCached(false);
                var mediaServer = MediaServerPrxHelper.checkedCast(obj);

                if (mediaServer == null)
                {
                    throw new ApplicationException("[ERROR] Invalid proxy!");
                }

                string choice;
                bool isRunning = true;
                while (isRunning)
                {
                    Console.WriteLine("################################################################################");
                    Console.WriteLine("Choose an action from the following list:");
                    Console.WriteLine("\tlist   - List available songs in the library.");
                    Console.WriteLine("\tfind   - Find songs by Title or by Artist.");
                    Console.WriteLine("\tplay   - Play (Stream) a selected song.");
                    Console.WriteLine("\tpause  - Pause OR Unpause the currently playing (streaming) song.");
                    Console.WriteLine("\tstop   - Stop the playback (stream).");
                    Console.WriteLine("\tupload - Add new song to the library.");
                    Console.WriteLine("\tupdate - Edit existing song in the library.");
                    Console.WriteLine("\tdelete - Delete existing song from the library.");
                    Console.WriteLine("\tssl    - Toggle SSL feature 'on' or 'off'.");
                    Console.WriteLine("\tquit   - Quit");
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
                            PlaySong(mediaServer);
                            break;

                        case "pause":
                            PauseSong(mediaServer);
                            break;

                        case "stop":
                            StopSong(mediaServer);
                            break;

                        case "upload":
                            UploadSong(mediaServer);
                            break;

                        case "update":
                            UpdateSong(mediaServer);
                            break;

                        case "delete":
                            DeleteSong(mediaServer);
                            break;

                        case "ssl":
                            _secure = !_secure;
                            mediaServer = (MediaServerPrx)mediaServer.ice_secure(_secure);

                            if (_secure)
                            {
                                Console.WriteLine("[INFO] SSL is now enabled.");
                            }
                            else
                            {
                                Console.WriteLine("[INFO] SSL is now disabled.");
                            }
                            break;

                        case "quit":
                        case "exit":
                            isRunning = false;
                            break;

                        default:
                            Console.WriteLine("[ERROR] That action doesn't exist...");
                            break;
                    };

                    Console.WriteLine("################################################################################");
                    Console.WriteLine();
                }
            }
            finally
            {
                communicator.destroy();
            }
        }

        private void DisplayAvailableSongs(MediaServerPrx mediaServer)
        {
            var songs = mediaServer.GetAllSongs(_clientId.ToString());
            foreach (Song song in songs)
            {
                Console.WriteLine($"  * {song.Title} - {song.Artist} ({song.Id})");
            }
        }

        private void FindSongs(MediaServerPrx mediaServer)
        {
            Console.Write("> Enter search query for song title or artist name: ");
            var choice = Console.ReadLine();
            Console.WriteLine();

            var songs = mediaServer.FindSongs(_clientId.ToString(), choice);
            foreach (Song song in songs)
            {
                Console.WriteLine($"  * {song.Title} - {song.Artist} ({song.Id})");
            }
        }

        private void PlaySong(MediaServerPrx mediaServer)
        {
            DisplayAvailableSongs(mediaServer);

            Console.WriteLine();
            Console.Write("> Enter filename of the song that you want to PLAY: ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            var url = mediaServer?.PlaySong(_clientId.ToString(), choice);
            if (!string.IsNullOrEmpty(url))
            {
                if (_media != null)
                {
                    _media.Dispose();
                }

                _media = new Media(_vlc, new Uri(url));
                _player.Play(_media);
            }
            else
            {
                Console.WriteLine($"[ERROR] Could not play the song '{choice}'. Please try again.");
            }
        }

        private void PauseSong(MediaServerPrx mediaServer)
        {
            mediaServer?.PauseSong(_clientId.ToString());

            if (_player.State == VLCState.Paused)
            {
                _player.Play();
            }
            else
            {
                _player.Pause();
            }
        }

        private void StopSong(MediaServerPrx mediaServer)
        {
            mediaServer?.StopSong(_clientId.ToString());
            _player.Stop();
            _logger.Information($"[{_clientId}] LibVLC player is currently in '{_player.State}' state.");
        }

        private void UploadSong(MediaServerPrx mediaServer)
        {
            Console.Write("> Enter path to the new song's local file: ");
            string filepath = Console.ReadLine();
            Console.WriteLine();

            if (File.Exists(filepath))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        string filename = Path.GetFileName(filepath);
                        const int chunkSize = 8192;
                        byte[] chunk = new byte[chunkSize];
                        using var fs = File.OpenRead(filepath);

                        while (fs.Position < fs.Length)
                        {
                            fs.Read(chunk, 0, chunkSize);

                            await mediaServer.UploadSongChunkAsync(_clientId.ToString(), filename, (int)fs.Position - chunk.Length, chunk);

                            //string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                            //Console.WriteLine(utfString);
                        }

                        Console.WriteLine($"Song upload complete for '{filename}'!");
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Exception has occured!");
                    }
                });
            }
            else
            {
                Console.WriteLine($"[ERROR] Could not find the song file at path '{filepath}'. Please try again.");
            }
        }

        private void UpdateSong(MediaServerPrx mediaServer)
        {
            DisplayAvailableSongs(mediaServer);

            Console.WriteLine();
            Console.Write("> Enter filename of the song that you want to UPDATE: ");
            string filename = Console.ReadLine();
            Console.Write("> Enter updated 'title' for the song: ");
            string title = Console.ReadLine();
            Console.Write("> Enter updated 'artist' for the song: ");
            string artist = Console.ReadLine();
            Console.WriteLine();

            var result = mediaServer?.UpdateSong(
                _clientId.ToString(), 
                new Song() { 
                    Id = (string.IsNullOrEmpty(filename) ? "Test.mp3" : filename), 
                    Title = title, 
                    Artist = artist 
                }
            );

            if (result == true)
            {
                Console.WriteLine("Successfully updated the song details!");
            }
            else
            {
                Console.WriteLine("Failed to update the song details...");
            }
        }

        private void DeleteSong(MediaServerPrx mediaServer)
        {
            DisplayAvailableSongs(mediaServer);

            Console.WriteLine();
            Console.Write("> Enter filename of the song that you want to DELETE: ");
            string filepath = Console.ReadLine();
            Console.WriteLine();

            var result = mediaServer?.DeleteSong(_clientId.ToString(), Path.GetFileName(filepath));

            if (result == true)
            {
                Console.WriteLine("Successfully deleted the song!");
            }
            else
            {
                Console.WriteLine("Failed to delete the song...");
            }
        }

        private void TestAsyncUploads(MediaServerPrx mediaServer)
        {
            var filename = "lorem.txt";
            var content = File.ReadAllBytes($"./local-lib/" + filename);
            var results = mediaServer.begin_UploadSong(_clientId.ToString(), filename, content);
            mediaServer.begin_UploadSong(_clientId.ToString(), filename, content);

            Task.Run(() =>
            {
                string filename = "lorem.txt";
                string filepath = $"./local-lib/{filename}";
                const int chunkSize = 1140;
                int offset = 0;
                using var fs = File.OpenRead(filepath);
                using var br = new BinaryReader(fs);

                while (br.PeekChar() != -1)
                {
                    byte[] chunk = br.ReadBytes(chunkSize);
                    mediaServer.UploadSongChunkAsync(_clientId.ToString(), filename, offset, chunk);
                    offset += chunk.Length;

                    string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                    Console.WriteLine(utfString);
                }
            });

            Task.Run(() =>
            {
                string filename = "lorem2.txt";
                string filepath = $"./local-lib/{filename}";
                const int chunkSize = 1026;
                int offset = 0;
                using var fs = File.OpenRead(filepath);
                using var br = new BinaryReader(fs);

                while (br.PeekChar() != -1)
                {
                    byte[] chunk = br.ReadBytes(chunkSize);
                    mediaServer.UploadSongChunkAsync(_clientId.ToString(), filename, offset, chunk);
                    offset += chunk.Length;

                    string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                    Console.WriteLine(utfString);
                }
            });

            Console.WriteLine("Test #1");
            Console.WriteLine("Test #2");
            Console.WriteLine("Test #3");

            mediaServer.end_UploadSong(results);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_media != null) _media.Dispose();
                    if (_player != null) _player.Dispose();
                    if (_vlc != null) _vlc.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
