using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VoxIA.Core.Media;

namespace VoxIA.ZerocIce.Core.Server
{
    public class MediaServer : MediaServerDisp_
    {
        private readonly List<int> _availablePorts = new List<int>();
        private readonly Mutex _mutex = new Mutex();
        private readonly Dictionary<string, LibVlcPlaybackService> _services = new Dictionary<string, LibVlcPlaybackService>();

        public MediaServer()
        {
            LibVLCSharp.Shared.Core.Initialize();

            _mutex.WaitOne();
            //TODO: Port range should be read from configurations!
            for (int i = 6000; i <= 6000; i++)
            {
                _availablePorts.Add(i);
            }
            _mutex.ReleaseMutex();
        }

        private int NextAvailablePort()
        {
            int port = -1;

            _mutex.WaitOne();
            if (_availablePorts.Count == 0)
            {
                port = _availablePorts[0];
                _availablePorts.RemoveAt(0);
            }
            _mutex.ReleaseMutex();
            
            return port;
        }

        public override Task<RegisterResponse> RegisterClientAsync(string clientId, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                return Task.FromResult(
                    new RegisterResponse(RegisterResult.AlreadyRegistered, string.Empty)
                );
            }
            else
            {
                int port = NextAvailablePort();
                if (port == -1)
                {
                    return Task.FromResult(
                        new RegisterResponse(RegisterResult.MaxClientsReached, string.Empty)
                    );
                }

                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");

                return Task.FromResult(
                    new RegisterResponse(RegisterResult.Success, $"http://{"192.168.0.11"}:{NextAvailablePort()}")
                );
            }
        }

        public override Task<bool> UnregisterClientAsync(string clientId, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];

                _mutex.WaitOne();
                _availablePorts.Add(6000);
                _mutex.ReleaseMutex();

                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public override async Task<Song[]> GetAllSongsAsync(Ice.Current current = null)
        {
            //TODO: Add more music extensions?
            var files = Directory.GetFiles($"tracks", "*.mp3");

            using var vlc = new LibVLC();

            List<Song> songs = new List<Song>();
            foreach (var file in files)
            {
                using var media = new Media(vlc, file, FromType.FromPath);
                await media.Parse();
                songs.Add(
                    //TODO: Could include AlbumCover here?
                    new Song()
                    {
                        Id = songs.Count.ToString(),
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

        public override async Task<bool> PlaySongAsync(string clientId, string filename, Ice.Current current = null)
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

            var song = new VoxIA.Core.Media.Song() { Url = filename };

            service.Playing += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Started the stream for client '{clientId}'.");
            service.Paused += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Paused the stream for client '{clientId}'.");
            service.Stopped += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Stopped the stream for client '{clientId}'.");

            await service.InitializeAsync(new VoxIA.Core.Streaming.Client(), song);
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

        public override Task UploadSongAsync(string filename, byte[] content, Ice.Current current = null)
        {
            return Task.Run(() => {
                //TODO: Remove hard-coded folder!
                File.WriteAllBytes("./tracks/" + filename, content);
            });
        }

        public override Task UploadSongChunkAsync(string filename, int offset, byte[] content, Ice.Current current = null)
        {
            return Task.Run(() =>
            {
                //string content = string.Empty;
                //{
                //    using MemoryStream ms = new(file);
                //    using StreamReader sr = new(ms);
                //    content = sr.ReadToEnd();
                //}

                //_mutex.WaitOne();
                //Console.WriteLine("###############################################################################");
                //Console.WriteLine($"      Filename : {filename}");
                //Console.WriteLine($"        Offset : {offset}");
                //Console.WriteLine($" Buffer Length : {content.Length}");
                //Console.WriteLine();
                //Console.WriteLine($"       Content : {content}");
                //Console.WriteLine("###############################################################################");

                try
                {
                    //TODO: Remove hard-coded folder!
                    string filepath = $"./tracks/{filename}";
                    if (offset == 0)
                    {
                        File.Delete(filepath);
                        File.WriteAllBytes(filepath, content);
                    }
                    else
                    {
                        using FileStream fs = new FileStream(filepath, FileMode.Append);
                        using BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(content);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
                //_mutex.ReleaseMutex();
            });
        }

        public override Task<bool> UpdateSongAsync(Song song, Ice.Current current = null)
        {
            if (song == null)
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(song?.Url))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(song?.Title))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(song?.Artist))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            //TODO: Remove hard-coded folder!
            var filePath = "./tracks/" + song?.Url;
            if (!File.Exists(filePath))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            using var vlc = new LibVLC();
            using var media = new Media(vlc, filePath, FromType.FromPath);
            media.SetMeta(MetadataType.Title, song?.Title);
            media.SetMeta(MetadataType.Artist, song?.Artist);

            return Task.FromResult(media.SaveMeta());
        }

        public override bool DeleteSong(string filename, Ice.Current current = null)
        {
            //TODO: Remove hard-coded folder!
            string path = "./tracks/" + filename;
            if (!File.Exists(path))
            {
                //TODO: Log an error message!
                return false;
            }

            File.Delete(path);
            return true;
        }
    }
}
