using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
            for (int i = 6000; i <= 6001; i++)
            {
                _availablePorts.Add(i);
            }
            _mutex.ReleaseMutex();
        }

        private int NextAvailablePort()
        {
            int port = -1;

            _mutex.WaitOne();
            if (_availablePorts.Count > 0)
            {
                port = _availablePorts[0];
                _availablePorts.RemoveAt(0);
            }
            _mutex.ReleaseMutex();
            
            return port;
        }

        public override Task<RegisterResponse> RegisterClientAsync(string clientId, Ice.Current current = null)
        {
            return Task.FromResult<RegisterResponse>(null);
        }

        public override Task<bool> UnregisterClientAsync(string clientId, Ice.Current current = null)
        {
            return Task.FromResult(false);
        }

        public LibVlcPlaybackService GetPlaybackService(string clientId)
        {
            // Existing client, return previously used service.
            if (_services.ContainsKey(clientId))
            {
                return _services[clientId];
            }
            // New client, return newly created service.
            else
            {
                var port = NextAvailablePort();

                // Port available, use a new port assignment for this client.
                if (port > 0)
                {
                    var service = _services[clientId] = new LibVlcPlaybackService(false, "--no-video");
                    service.Port = port;
                    return service;
                }
                // Port unavailable, try to reuse existing service...
                else
                {
                    var service = _services.Where(_ => _.Value.IsAvailable).FirstOrDefault();

                    // Recyclable playback service found, reuse it!
                    if (!string.IsNullOrEmpty(service.Key))
                    {
                        _services[clientId] = service.Value;
                        _services.Remove(service.Key);
                        return service.Value;
                    }
                    // All ports and playback services are busy...
                    else
                    {
                        Console.WriteLine("[ERROR] Server too busy. Cannot assign port for streaming...");
                        return null;
                    }

                }
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
                        Id = Path.GetFileName(file),
                        Title = media.Meta(MetadataType.Title),
                        Artist = media.Meta(MetadataType.Artist)
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

        public override async Task<string> PlaySongAsync(string clientId, string filename, Ice.Current current = null)
        {
            _mutex.WaitOne();
            LibVlcPlaybackService service = GetPlaybackService(clientId);
            _mutex.ReleaseMutex();

            if (service != null)
            {
                service.LengthChanged += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Length '{e.Length}'.");
                service.Playing += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Started the stream for client '{clientId}'.");
                service.Paused += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Paused the stream for client '{clientId}'.");
                service.Stopped += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Stopped the stream for client '{clientId}'.");

                //var ipAddress = Environment.GetEnvironmentVariable("DOCKER_HOST_IP");

                var url = await service?.PlayAsync(new VoxIA.Core.Streaming.Client(), filename);
                return url;
            }
            else
            {
                return null;
            }
        }

        public override bool PauseSong(string clientId, Ice.Current current = null)
        {
            _mutex.WaitOne();
            LibVlcPlaybackService service = GetPlaybackService(clientId);
            _mutex.ReleaseMutex();

            service?.Pause();

            return true;
        }

        public override bool StopSong(string clientId, Ice.Current current = null)
        {
            _mutex.WaitOne();
            LibVlcPlaybackService service = GetPlaybackService(clientId);
            _mutex.ReleaseMutex();

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

            if (string.IsNullOrEmpty(song?.Id))
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
            var filePath = "./tracks/" + song?.Id;
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
