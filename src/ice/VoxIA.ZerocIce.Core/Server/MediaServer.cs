using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Server
{
    public class MediaServer : MediaServerDisp_
    {
        private readonly Mutex _mutex = new();
        private readonly Dictionary<string, LibVlcPlaybackService> _services = new();

        public MediaServer()
        {
            LibVLCSharp.Shared.Core.Initialize();
        }

        public override async Task<Song[]> GetAllSongsAsync(Ice.Current current = null)
        {
            var files = Directory.GetFiles($".\\tracks");

            using var vlc = new LibVLC();

            List<Song> songs = new();
            foreach (var file in files)
            {
                using var media = new Media(vlc, file, FromType.FromPath);
                await media.Parse();
                songs.Add(
                    new Song()
                    {
                        Title = media.Meta(MetadataType.Title),
                        Artist = media.Meta(MetadataType.Artist),
                        Url = Path.GetFileName(file)
                    }
                );
            }

            return songs.ToArray();
        }

        public override async Task<Song[]> FindSongsAsync(string query, Ice.Current current = null)
        {
            string sanitizedQuery = query.Trim().ToLower();

            var songs = new List<Song>(await GetAllSongsAsync());
            return songs.FindAll(_ => {
                return
                    _.Title.ToLower().Contains(sanitizedQuery) ||
                    _.Artist.ToLower().Contains(sanitizedQuery);
            }).ToArray();
        }

        public override string[] FindSongsByTitle(string title, Ice.Current current = null)
        {
            throw new NotImplementedException();
        }

        public override string[] FindSongsByArtist(string artist, Ice.Current current = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> PlaySongAsync(string clientId, string songUrl, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];
            }
            else
            {
                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");
            }

            var song = new Song() { Url = songUrl };

            service.Playing += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Started the stream for client '{clientId}'.");
            service.Paused += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Paused the stream for client '{clientId}'.");
            service.Stopped += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Stopped the stream for client '{clientId}'.");

            await service.InitializeAsync(new IceClient(), song);
            return service?.Play() == true;
        }

        public override bool PauseSong(string clientId, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];
            }
            else
            {
                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");
            }

            service?.Pause();

            return true;
        }

        public override bool StopSong(string clientId, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];
            }
            else
            {
                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");
            }

            service?.Stop();

            return true;
        }

        public override bool AddSong(Song song, Ice.Current current = null)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateSong(Song song, Ice.Current current = null)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteSong(string songUrl, Ice.Current current = null)
        {
            throw new NotImplementedException();
        }

        public override string getLibraryContent(Ice.Current current = null)
        {
            var files = Directory.GetFiles($".\\stream-lib");

            StringBuilder sb = new();
            foreach (var f in files)
            {
                sb.AppendLine(f);
            }

            return sb.ToString();
        }

        public override void printString(string s, Ice.Current current = null)
        {
            Console.WriteLine(s);
        }

        public override Task uploadFileAsync(byte[] file, Ice.Current current = null)
        {
            return Task.Run(() => {
                MemoryStream ms = new(file);
                StreamReader sr = new(ms);
                var content = sr.ReadToEnd();

                File.WriteAllBytes("./upload-area/uploaded.txt", file);

                Thread.Sleep(5000);

                _mutex.WaitOne();
                Console.WriteLine(content);
                _mutex.ReleaseMutex();
            });
        }

        public override Task uploadFileChunkAsync(string filename, int offset, byte[] file, Ice.Current current = null)
        {
            return Task.Run(() =>
            {
                string content = string.Empty;
                {
                    using MemoryStream ms = new(file);
                    using StreamReader sr = new(ms);
                    content = sr.ReadToEnd();
                }

                _mutex.WaitOne();
                Console.WriteLine("###############################################################################");
                Console.WriteLine($"      Filename : {filename}");
                Console.WriteLine($"        Offset : {offset}");
                Console.WriteLine($" Buffer Length : {file.Length}");
                Console.WriteLine();
                Console.WriteLine($"       Content : {content}");
                Console.WriteLine("###############################################################################");

                try
                {
                    string filepath = $"./upload-area/{filename}";
                    if (offset == 0)
                    {
                        File.Delete(filepath);
                        File.WriteAllBytes(filepath, file);
                    }
                    else
                    {
                        using FileStream fs = new(filepath, FileMode.Append);
                        using BinaryWriter bw = new(fs);
                        bw.Write(file);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
                _mutex.ReleaseMutex();
            });
        }
    }
}
