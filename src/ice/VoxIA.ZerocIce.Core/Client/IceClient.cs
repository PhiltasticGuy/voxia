﻿using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Client
{
    public class IceClient : IIceClient, IDisposable
    {
        private readonly LibVLC _vlc;
        private readonly MediaPlayer _player;
        private readonly Media _media;

        private bool disposedValue;

        public IceClient() : this(false, "--no-video")
        {
        }

        public IceClient(bool enableDebugLogs, params string[] options)
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC(enableDebugLogs, options);

            if (!enableDebugLogs)
            {
                _vlc.Log += (sender, e) => { /* Do nothing! Don't log in the console... */ };
            }

            _player = new MediaPlayer(_vlc);
            //TODO: Port must be assigned from server after registering!
            _media = new Media(_vlc, new Uri("http://localhost:6000/stream.mp3"));
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
                var obj = communicator.stringToProxy("SimplePrinter:tcp -h 127.0.0.1 -p 10000");
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
                    Console.WriteLine("\tlist   - List available songs in the library.");
                    Console.WriteLine("\tfind   - Find songs by Title or by Artist.");
                    Console.WriteLine("\tplay   - Play (Stream) the selected song.");
                    Console.WriteLine("\tpause  - Pause the currently playing (streaming) song.");
                    Console.WriteLine("\tstop   - Stop the playback (stream).");
                    Console.WriteLine("\tupload - Add new song to the library.");
                    Console.WriteLine("\tupdate - Edit existing song in the library.");
                    Console.WriteLine("\tdelete - Delete existing song from the library.");
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
            }
            finally
            {
                communicator.destroy();
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

        private void PlaySong(MediaServerPrx mediaServer)
        {
            DisplayAvailableSongs(mediaServer);

            Console.WriteLine();
            Console.Write("> Enter filename of the song that you want to PLAY: ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            if (mediaServer?.PlaySong("1", choice) == true)
            {
                _player.Play(_media);
            }
            else
            {
                Console.WriteLine($"Could not play the song '{choice}'. Please try again.");
            }
        }

        private void PauseSong(MediaServerPrx mediaServer)
        {
            mediaServer?.PauseSong("1");
        }

        private void StopSong(MediaServerPrx mediaServer)
        {
            mediaServer?.StopSong("1");
        }

        private void UploadSong(MediaServerPrx mediaServer)
        {
            Console.Write("> Enter path to the new song's local file: ");
            string filepath = Console.ReadLine();
            Console.WriteLine();

            Task.Run(async () =>
            {
                string filename = Path.GetFileName(filepath);
                const int chunkSize = 2048;
                int offset = 0;
                using var fs = File.OpenRead(filepath);
                using var br = new BinaryReader(fs);

                while (br.PeekChar() != -1)
                {
                    byte[] chunk = br.ReadBytes(chunkSize);
                    await mediaServer.UploadSongChunkAsync(filename, offset, chunk);
                    offset += chunk.Length;

                    //string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                    //Console.WriteLine(utfString);
                }
            });
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

            mediaServer?.UpdateSong(new Song() { Title = title, Artist = artist, Url = "Test.mp3" });
        }

        private void DeleteSong(MediaServerPrx mediaServer)
        {
            DisplayAvailableSongs(mediaServer);

            Console.WriteLine();
            Console.Write("> Enter filename of the song that you want to DELETE: ");
            string filepath = Console.ReadLine();
            Console.WriteLine();

            mediaServer?.DeleteSong(Path.GetFileName(filepath));
        }

        private void TestAsyncUploads(MediaServerPrx mediaServer)
        {
            var filename = "lorem.txt";
            var content = File.ReadAllBytes($"./local-lib/" + filename);
            var results = mediaServer.begin_UploadSong(filename, content);
            mediaServer.begin_UploadSong(filename, content);

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
                    mediaServer.UploadSongChunkAsync(filename, offset, chunk);
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
                    mediaServer.UploadSongChunkAsync(filename, offset, chunk);
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}